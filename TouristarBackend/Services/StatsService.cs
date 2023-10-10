using TouristarBackend.Contracts;
using TouristarModels.Enums;
using TouristarModels.Models;

namespace TouristarBackend.Services;

public class StatsService : IStatsService
{
    private IRepositoryManager _repository;

    public StatsService(IRepositoryManager repository)
    {
        _repository = repository;
    }

    public UserStatsDto GetUserStats(long userId)
    {
        // Retrieve activities.
        var activities = _repository.Activity
            .FindAllActivities(userId)
            .Where(x => x.Timestamp > DateTime.UtcNow);

        // Retrieve unique cities visited.
        var trips = _repository.Trip.FindAllTrips(userId).Where(x => x.DepartAt > DateTime.UtcNow);
        var cities = trips.Select(x => x.ArrivalLocation.City).Distinct();

        // Retrieve unique countries visited.
        var countries = trips.Select(x => x.ArrivalLocation.Country).Distinct();

        // Retrieve unique airports visited.
        var airports = new List<string>();
        foreach (Trip trip in trips)
        {
            var outboundTickets = trip.Tickets.Where(x => x.Leg == TicketLeg.Outbound);
            if (!outboundTickets.Any() || outboundTickets == null)
                continue;
            var ticket = outboundTickets.First();
            var airportCode = ticket.ArrivalAirportCode;
            if (!airports.Contains(airportCode))
            {
                airports.Add(airportCode);
            }
        }

        return new UserStatsDto
        {
            Activities = activities.Count(),
            Cities = cities.Count(),
            Countries = countries.Count(),
            Airports = airports.Count()
        };
    }
}
