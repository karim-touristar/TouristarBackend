using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface ILocationService
{
    Task<IEnumerable<Location>> SearchForLocations(string query);
    public Task<Location> CreateOrReturnLocation(CreateLocationDto location);
}
