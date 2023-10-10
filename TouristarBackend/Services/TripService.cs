using TouristarModels.Constants;
using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Exceptions;

namespace TouristarBackend.Services;

public class TripService : ITripService
{
    private readonly IRepositoryManager _repository;
    private readonly ILocationService _service;
    private readonly ILogger<ITripService> _logger;

    public TripService(
        IRepositoryManager repository,
        ILocationService service,
        ILogger<ITripService> logger
    )
    {
        _repository = repository;
        _service = service;
        _logger = logger;
    }

    public IEnumerable<Trip> GetAllTrips(long userId)
    {
        var user = _repository.User.FindById(userId);
        var userTrips = _repository.Trip.FindAllTrips(userId);
        var invitedTrips = _repository.Trip
            .FindInvitedTrips(user.Email)
            .Select(x =>
            {
                var inviterUserId = x.TripInvitations.First().UserId;
                if (inviterUserId == userId)
                    return x;
                var inviter = _repository.User.FindById(inviterUserId);
                var transformedTrip = x;
                transformedTrip.InviterName = inviter.Name;
                return transformedTrip;
            });
        return userTrips.Concat(invitedTrips).ToList();
    }

    public Trip GetTrip(long tripId) => _repository.Trip.FindTrip(tripId);

    public async Task<Trip> CreateTrip(long userId, TripCreateRequestDto trip)
    {
        var departureLocation = await _service.CreateOrReturnLocation(trip.DepartureLocation);
        var arrivalLocation = await _service.CreateOrReturnLocation(trip.ArrivalLocation);

        Trip transformedTrip =
            new()
            {
                UserId = userId,
                DepartAt = TimeZoneInfo.ConvertTimeToUtc(trip.DepartAt),
                ReturnAt = TimeZoneInfo.ConvertTimeToUtc(trip.ReturnAt),
                DepartureLocationId = departureLocation.Id,
                ArrivalLocationId = arrivalLocation.Id
            };
        _repository.Trip.CreateTrip(transformedTrip);
        await _repository.Save();

        await FetchAndSaveLocationImage(transformedTrip.ArrivalLocation);

        // Re-fetch the trip to return populated relationship fields.
        var createdTrip = _repository.Trip.FindTrip(transformedTrip.Id);
        if (createdTrip == null)
            throw new Exception("Created trip could not be found in the db.");
        return createdTrip;
    }

    private async Task FetchAndSaveLocationImage(Location location)
    {
        // Fetch and save destination image.
        try
        {
            var query = $"{location.City} {location.Country}";
            var image = await _repository.GooglePlaces.GetLocationImage(query);
            var fileName = $"{location.Id}.jpg";
            await _repository.Storage.UploadFile(BucketNames.LocationImages, image, fileName);
            // TODO: update location logoUrl so this doesn't get overfetched.
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                $"Could not retrieve location image, location id: {location.Id}. Exception {e}"
            );
        }
    }

    public async Task<Trip> UpdateTrip(TripUpdateRequestDto trip, long tripId)
    {
        var tripToUpdate = _repository.Trip.FindTrip(tripId);
        if (tripToUpdate == null)
            throw new NotFoundException("Trip to update could not be found.");

        if (trip.DepartAt != null)
        {
            tripToUpdate.DepartAt = TimeZoneInfo.ConvertTimeToUtc(trip.DepartAt.Value);
        }
        if (trip.ReturnAt != null)
        {
            tripToUpdate.ReturnAt = TimeZoneInfo.ConvertTimeToUtc(trip.ReturnAt.Value);
        }

        if (trip.ArrivalLocation != null)
        {
            var arrivalLocation = await _service.CreateOrReturnLocation(trip.ArrivalLocation);
            tripToUpdate.ArrivalLocation = arrivalLocation;
            await FetchAndSaveLocationImage(arrivalLocation);
        }
        if (trip.DepartureLocation != null)
        {
            var departureLocation = await _service.CreateOrReturnLocation(trip.DepartureLocation);
            tripToUpdate.DepartureLocation = departureLocation;
        }

        await _repository.Save();

        var updatedTrip = _repository.Trip.FindTrip(tripToUpdate.Id);
        if (updatedTrip == null)
            throw new NotFoundException("Could not find updated trip.");
        return updatedTrip;
    }

    public async Task<bool> DeleteTrip(long tripId)
    {
        var trip = _repository.Trip.FindTrip(tripId);
        if (trip == null)
            throw new Exception("Trip could not be found.");
        _repository.Trip.DeleteTrip(trip);
        await _repository.Save();
        return true;
    }
}
