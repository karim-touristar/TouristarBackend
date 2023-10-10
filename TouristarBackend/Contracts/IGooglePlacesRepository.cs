using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface IGooglePlacesRepository
{
    Task<byte[]> GetLocationImage(string query);
    Task<IEnumerable<ActivityLocation>?> FindActivityLocations(string query, Location tripLocation);
    Task<GoogleGeometryResponseDto> FindGeometry(string placeId);
    Task<byte[]> GetMapsImage(float latitude, float longitude);
}
