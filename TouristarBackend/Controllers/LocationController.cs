using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristarModels.Models;
using TouristarBackend.Contracts;

namespace TouristarBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LocationController : ControllerBase
{
    private readonly ILocationService _service;

    public LocationController(ILocationService service)
    {
        _service = service;
    }

    // GET: api/Location
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Location>>> GetLocationByQueryString(
        [FromQuery] string query
    )
    {
        var result = await _service.SearchForLocations(query);
        return Ok(result);
    }
}
