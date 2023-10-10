using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface IActivityLocationService
{
    Task<IEnumerable<ActivityLocation>> FindLocationsByQuery(string query, long tripId);
}