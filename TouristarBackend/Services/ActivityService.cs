using Microsoft.EntityFrameworkCore;
using TouristarModels.Constants;
using TouristarModels.Enums;
using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Exceptions;

namespace TouristarBackend.Services;

public class ActivityService : IActivityService
{
    private readonly IRepositoryManager _repository;
    private readonly ILogger<IActivityService> _logger;

    public ActivityService(IRepositoryManager repository, ILogger<IActivityService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IEnumerable<Activity> GetAllActivities(long userId)
    {
        try
        {
            return _repository.Activity.FindAllActivities(userId);
        }
        catch (Exception e)
        {
            throw new Exception("There was an error fetching activities.", e);
        }
    }

    public IEnumerable<Activity> GetAllActivitiesForTrip(long userId, long tripId)
    {
        try
        {
            return _repository.Activity.FindAllActivitiesForTrip(userId, tripId);
        }
        catch (Exception e)
        {
            throw new Exception("There was an error fetching activities.", e);
        }
    }

    public Activity GetActivity(long activityId)
    {
        try
        {
            return _repository.Activity.FindActivity(activityId);
        }
        catch (Exception e)
        {
            throw new Exception("There was an error fetching activity.", e);
        }
    }

    private async Task<ActivityLocation> GetActivityLocation(CreateActivityLocationDto location)
    {
        GeometryResult geometry = (
            await _repository.GooglePlaces.FindGeometry(location.PlacesId)
        ).Results.First();
        return new ActivityLocation
        {
            MainText = location.MainText,
            SecondaryText = location.SecondaryText,
            Description = location.Description,
            Address1 = location.Address1,
            Address2 = location.Address2,
            Address3 = location.Address3,
            Address4 = location.Address4,
            Latitude = geometry.Geometry.Location.Lat,
            Longitude = geometry.Geometry.Location.Lng,
            PlacesId = location.PlacesId,
        };
    }

    public async Task<Activity> CreateActivity(long userId, ActivityCreateDto activity)
    {
        try
        {
            // Check if trip exists.
            var trip = _repository.Trip.FindTrip(activity.TripId);

            var location = await GetActivityLocation(activity.Location);

            var activityType = ComputeActivityType(activity.Location.Types);

            Activity transformedActivity =
                new()
                {
                    UserId = userId,
                    Name = activity.Name,
                    Description = activity.Description,
                    Notes = activity.Notes,
                    Location = location,
                    TripId = activity.TripId,
                    Timestamp = TimeZoneInfo.ConvertTimeToUtc(activity.Timestamp),
                    Type = activityType,
                    Documents = activity.Documents
                        .Select(
                            doc =>
                                new Document
                                {
                                    Name = doc.Name,
                                    FilePath = doc.FilePath,
                                    FileName = doc.FileName,
                                    Type = doc.Type,
                                    UserId = userId,
                                    TripId = activity.TripId,
                                }
                        )
                        .ToList(),
                };
            _repository.Activity.CreateActivity(transformedActivity);

            await _repository.Save();

            try
            {
                if (activityType == ActivityType.Unknown)
                {
                    await CreateAndUploadActivityMapImage(location, userId, transformedActivity.Id);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"There was an issue creating activity image for activity {transformedActivity.Id}. Exception {e}"
                );
            }

            // Re-fetch the entity to return populated relationship fields.
            var createdActivity = _repository.Activity.FindActivity(transformedActivity.Id);
            if (createdActivity == null)
                throw new NotFoundException("Created activity could not be found in the db.");
            return createdActivity;
        }
        catch (Exception e)
        {
            throw new Exception("There was an issue creating activity.", e);
        }
    }

    private async Task CreateAndUploadActivityMapImage(
        ActivityLocation activityLocation,
        long userId,
        long activityId
    )
    {
        if (activityLocation.Latitude == null || activityLocation.Longitude == null)
        {
            return;
        }
        var image =
            await _repository.GooglePlaces.GetMapsImage(
                (float)activityLocation.Latitude,
                (float)activityLocation.Longitude
            )
            ?? throw new InvalidOperationException(
                $"Could not save activity image. User id: {userId}"
            );
        var fileName = $"{activityId}.jpg";
        await _repository.Storage.UploadFile(BucketNames.ActivityImages, image, fileName);
    }

    public async Task<Activity> UpdateActivity(ActivityUpdateDto activity, long activityId)
    {
        try
        {
            var activityToUpdate =
                _repository.Activity.FindActivity(activityId)
                ?? throw new NotFoundException("Activity to update could not be found.");

            ActivityLocation? newLocation = null;
            ActivityType? newActivityType = null;
            if (
                activity.Location != null
                && activity.Location.PlacesId != activityToUpdate.Location.PlacesId
            )
            {
                newLocation = await GetActivityLocation(activity.Location);
                newActivityType = ComputeActivityType(activity.Location.Types);

                try
                {
                    if (newActivityType == ActivityType.Unknown)
                    {
                        await CreateAndUploadActivityMapImage(
                            newLocation,
                            activityToUpdate.UserId,
                            activityToUpdate.Id
                        );
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        $"There was an issue creating activity image for activity {activityToUpdate.Id}. Exception {e}"
                    );
                }
            }

            activityToUpdate.Name = activity.Name ?? activityToUpdate.Name;
            activityToUpdate.Description = activity.Description ?? activityToUpdate.Description;
            activityToUpdate.Notes = activity.Notes ?? activityToUpdate.Notes;
            if (activity.Timestamp != null)
            {
                activityToUpdate.Timestamp = TimeZoneInfo.ConvertTimeToUtc(
                    activity.Timestamp.Value
                );
            }
            activityToUpdate.Location = newLocation ?? activityToUpdate.Location;
            activityToUpdate.Type = newActivityType ?? activityToUpdate.Type;
            activityToUpdate.Trip = null!;

            if (activity.Documents.Any())
            {
                activityToUpdate.Documents = activity.Documents
                    .Select(
                        doc =>
                            new Document
                            {
                                Name = doc.Name,
                                FilePath = doc.FilePath,
                                FileName = doc.FileName,
                                Type = doc.Type,
                                UserId = activityToUpdate.UserId,
                                TripId = activityToUpdate.TripId,
                            }
                    )
                    .ToList();
            }

            _repository.Activity.UpdateActivity(activityToUpdate);
            await _repository.Save();

            var updatedActivity =
                _repository.Activity.FindActivity(activityToUpdate.Id)
                ?? throw new NotFoundException("Could not find updated activity.");
            return updatedActivity;
        }
        catch (Exception e)
        {
            throw new DbUpdateException("There was an issue updating activity.", e);
        }
    }

    public async Task<bool> DeleteActivity(long activityId)
    {
        try
        {
            var activity = _repository.Activity.FindActivity(activityId);
            _repository.Activity.DeleteActivity(activity);
            await _repository.Save();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception("There was an issue deleting activity.", e);
        }
    }

    private ActivityType ComputeActivityType(IEnumerable<string> Types)
    {
        if (!Types.Any())
            return ActivityType.Unknown;
        if (Types.Contains("food"))
        {
            return ActivityType.Food;
        }
        else if (Types.Contains("restaurant"))
        {
            return ActivityType.Food;
        }
        return ActivityType.Unknown;
    }
}
