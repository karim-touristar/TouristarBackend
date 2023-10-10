using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristarModels.Models;
using TouristarBackend.Contracts;

namespace TouristarBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ActivityLocationController : Controller
{
    private readonly IActivityLocationService _service;

    public ActivityLocationController(IActivityLocationService service)
    {
        _service = service;
    }

    // GET: api/ActivityLocation
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActivityLocation>>> GetLocationByQueryString([FromQuery] string query,
        [FromQuery] long tripId)
    {
        var result = await _service.FindLocationsByQuery(query, tripId);
        return Ok(result);
    }
}