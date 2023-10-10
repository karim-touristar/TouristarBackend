using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface ITicketService
{
    IEnumerable<Ticket> GetTicketsForTrip(long tripId);
}