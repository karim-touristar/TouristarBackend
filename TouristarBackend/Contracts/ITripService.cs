using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface ITripService
{
    IEnumerable<Trip> GetAllTrips(long userId);
    Trip? GetTrip(long tripId);
    Task<Trip> CreateTrip(long userId, TripCreateRequestDto trip);
    Task<Trip> UpdateTrip(TripUpdateRequestDto trip, long tripId);
    Task<bool> DeleteTrip(long tripId);
}