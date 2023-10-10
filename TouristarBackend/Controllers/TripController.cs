using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Helpers;

namespace TouristarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Trip>> Get()
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = _tripService.GetAllTrips(userId);
            return Ok(result);
        }

        // GET: api/Trip/5
        [HttpGet("{id}")]
        public ActionResult<Trip> Get(long id)
        {
            var result = _tripService.GetTrip(id);
            return Ok(result);
        }

        // POST: api/Trip
        [HttpPost]
        public async Task<ActionResult<Trip>> Post(TripCreateRequestDto trip)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = await _tripService.CreateTrip(userId, trip);
            return Ok(result);
        }

        // PUT: api/Trip/5
        [HttpPatch("{id}")]
        public async Task<ActionResult<Trip>> Put(long id, TripUpdateRequestDto trip)
        {
            var result = await _tripService.UpdateTrip(trip, id);
            return Ok(result);
        }

        // DELETE: api/Trip/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _tripService.DeleteTrip(id);
            return Ok(result);
        }
    }
}
