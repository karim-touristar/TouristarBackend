using Microsoft.AspNetCore.Mvc;
using TouristarModels.Models;
using TouristarBackend.Contracts;

namespace TouristarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _service;

        public TicketController(ITicketService service)
        {
            _service = service;
        }

        [HttpGet("trip/{tripId}")]
        public ActionResult<IEnumerable<Ticket>> GetTicketsForTrip(long tripId)
        {
            var tickets = _service.GetTicketsForTrip(tripId);
            return Ok(tickets);
        }
    }
}