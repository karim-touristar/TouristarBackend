using Microsoft.AspNetCore.Mvc;
using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Helpers;

namespace TouristarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChecklistController : ControllerBase
    {
        private readonly IChecklistService _service;

        public ChecklistController(IChecklistService service)
        {
            _service = service;
        }

        // GET: api/Checklist/trip/5
        [HttpGet("trip/{tripId}")]
        public ActionResult<IEnumerable<Checklist>> GetChecklistsForTrip(long tripId)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = _service.GetAllChecklistsForTrip(userId, tripId);
            return Ok(result);
        }

        // GET: api/Checklist/5
        [HttpGet("{id}")]
        public ActionResult<Checklist> GetChecklist(int id)
        {
            var result = _service.GetChecklist(id);
            return Ok(result);
        }

        // POST: api/Checklist
        [HttpPost]
        public async Task<ActionResult<Checklist>> PostChecklist([FromBody] ChecklistCreateDto checklist)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = await _service.CreateChecklist(userId, checklist);
            return Ok(result);
        }

        // PATCH: api/Checklist/5
        [HttpPatch("{id}")]
        public async Task<ActionResult<Checklist>> PatchChecklist(int id, [FromBody] ChecklistUpdateDto checklist)
        {
            var result = await _service.UpdateChecklist(id, checklist);
            return Ok(result);
        }

        // DELETE: api/Checklist/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteChecklist(int id)
        {
            var result = await _service.DeleteChecklist(id);
            return Ok(result);
        }
    }
}