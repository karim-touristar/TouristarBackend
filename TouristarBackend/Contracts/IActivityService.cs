using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface IActivityService
{
    IEnumerable<Activity> GetAllActivities(long userId);
    IEnumerable<Activity> GetAllActivitiesForTrip(long userId, long tripId);
    Activity? GetActivity(long activityId);
    Task<Activity> CreateActivity(long userId, ActivityCreateDto trip);
    Task<Activity> UpdateActivity(ActivityUpdateDto activity, long tripId);
    Task<bool> DeleteActivity(long activityId);
}