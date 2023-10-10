using TouristarModels.Models;
using TouristarBackend.Contracts;

namespace TouristarBackend.Services;

public class TicketService : ITicketService
{
    private readonly ILogger<TicketService> _logger;
    private readonly IRepositoryManager _repository;

    public TicketService(ILogger<TicketService> logger, IRepositoryManager repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public IEnumerable<Ticket> GetTicketsForTrip(long tripId)
    {
        var tickets = _repository.Ticket.FindTicketsForTrip(tripId).ToList();
        if (!tickets.Any()) throw new InvalidOperationException($"Could not find tickets for trip {tripId}");
        return tickets;
    }
}