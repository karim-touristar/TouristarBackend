using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Helpers;
using TouristarBackend.Models;

namespace TouristarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _service;

        public ActivityController(IActivityService service)
        {
            _service = service;
        }

        // GET: api/Activity
        [HttpGet]
        public ActionResult<IEnumerable<Activity>> Get()
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = _service.GetAllActivities(userId) ?? throw new Exception("There was an issue fetching activities.");
            return Ok(result);
        }

        // GET: api/Activity/trip/5
        [HttpGet("trip/{tripId}")]
        public ActionResult<IEnumerable<Activity>> GetForTrip(long tripId)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = _service.GetAllActivitiesForTrip(userId, tripId);
            if (result == null)
            {
                throw new Exception("There was an issue fetching activities.");
            }

            return Ok(result);
        }

        // GET: api/Activity/5
        [HttpGet("{id}")]
        public ActionResult<Activity> Get(long id)
        {
    var result = _service.GetActivity(id);
            if (result == null)
            {
                throw new Exception("There was an issue fetching activity.");
            }

            return Ok(result);
        }

        // POST: api/Activity
        [HttpPost]
        public async Task<ActionResult<Activity>> Post([FromBody] ActivityCreateDto activity)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = await _service.CreateActivity(userId, activity);
            return Ok(result);
        }

        // PATCH: api/Activity/5
        [HttpPatch("{id}")]
        public async Task<ActionResult<Activity>> Patch(int id, [FromBody] ActivityUpdateDto activity)
        {
            var result = await _service.UpdateActivity(activity, id);
            return Ok(result);
        }

        // DELETE: api/Activity/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _service.DeleteActivity(id);
            return Ok(result);
        }
    }
}