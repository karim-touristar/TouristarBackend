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
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _service;

        public DocumentController(IDocumentService service)
        {
            _service = service;
        }

        // GET: api/Document
        [HttpGet("trip/{tripId}", Name = "Get documents for trip")]
        public ActionResult<IEnumerable<Document>> GetForTrip(long tripId)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = _service.GetAllDocumentsForTrip(userId, tripId);
            return Ok(result);
        }

        [HttpGet("activity/{activityId}", Name = "Get documents for activity")]
        public ActionResult<IEnumerable<Document>> GetForActivity(long activityId)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = _service.GetAllDocumentsForActivity(userId, activityId);
            return Ok(result);
        }

        // GET: api/Document/5
        [HttpGet("{id}")]
        public ActionResult<Document> Get(int id)
        {
            var result = _service.GetDocument(id);
            return Ok(result);
        }

        // POST: api/Document
        [HttpPost]
        public async Task<ActionResult<Document>> Post([FromBody] DocumentCreateDto document)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = await _service.CreateDocument(userId, document);
            return Ok(result);
        }

        // DELETE: api/Document/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _service.DeleteDocument(id);
            return Ok(result);
        }
    }
}