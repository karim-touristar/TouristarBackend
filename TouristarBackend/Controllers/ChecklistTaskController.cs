using Microsoft.AspNetCore.Mvc;
using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Helpers;

namespace TouristarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChecklistTaskController : ControllerBase
    {
        private readonly IChecklistTaskService _service;

        public ChecklistTaskController(IChecklistTaskService service)
        {
            _service = service;
        }

        // GET: api/ChecklistTask/checklist/5
        [HttpGet("checklist/{checklistId}")]
        public ActionResult<IEnumerable<ChecklistTask>> GetForChecklist(int checklistId)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = _service.GetAllTasksForChecklist(userId, checklistId);
            return Ok(result);
        }

        // POST: api/ChecklistTask
        [HttpPost]
        public async Task<ActionResult<ChecklistTask>> PostTask([FromBody] ChecklistTaskCreateDto task)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = await _service.CreateChecklistTask(userId, task);
            return Ok(result);
        }

        // PATCH: api/ChecklistTask/5
        [HttpPatch("{id}")]
        public async Task<ActionResult<ChecklistTask>> Put(int id, [FromBody] ChecklistTaskUpdateDto task)
        {
            var result = await _service.UpdateChecklistTask(id, task);
            return Ok(result);
        }

        // DELETE: api/ChecklistTask/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _service.DeleteChecklistTask(id);
            return Ok(result);
        }
    }
}