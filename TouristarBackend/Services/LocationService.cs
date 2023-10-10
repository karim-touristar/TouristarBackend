using TouristarModels.Models;
using TouristarBackend.Contracts;

namespace TouristarBackend.Services;

public class LocationService : ILocationService
{
    private readonly IRepositoryManager _repository;

    public LocationService(IRepositoryManager repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Location>> SearchForLocations(string query)
    {
        return await _repository.Radar.SearchLocations(query);
    }

    public async Task<Location> CreateOrReturnLocation(CreateLocationDto location)
    {
        try
        {
            return _repository.Location.FindByCountryAndCity(location.City, location.Country);
        }
        catch
        {
            Location locationToCreate =
                new()
                {
                    City = location.City,
                    Country = location.Country,
                    CountryCode = location.CountryCode,
                    State = location.State,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Emoji = location.Emoji
                };
            var locations = new List<Location> { locationToCreate };
            _repository.Location.CreateLocations(locations);
            await _repository.Save();
            return _repository.Location.FindByCountryAndCity(location.City, location.Country);
        }
    }
}
