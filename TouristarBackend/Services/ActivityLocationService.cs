using TouristarModels.Models;
using TouristarBackend.Contracts;

namespace TouristarBackend.Services;

public class ActivityLocationService : IActivityLocationService
{
    private readonly IRepositoryManager _repository;

    public ActivityLocationService(IRepositoryManager repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ActivityLocation>> FindLocationsByQuery(string query, long tripId)
    {
        var trip = _repository.Trip.FindTrip(tripId);
        if (trip.ArrivalLocation == null)
        {
            throw new InvalidOperationException(
                $"Could not retrieve trip or arrival location for trip id: {trip.Id}, user: {trip.UserId}.");
        }

        var result = await _repository.GooglePlaces.FindActivityLocations(query, trip.ArrivalLocation);
        if (result == null)
        {
            throw new InvalidOperationException(
                $"Could not find activity locations for query {query}, location: {trip.ArrivalLocation.Id}, trip: {trip.Id}, user: {trip.UserId}.");
        }

        return result;
    }
}