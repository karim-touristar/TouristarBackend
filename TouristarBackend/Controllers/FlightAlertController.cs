using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristarBackend.Contracts;

namespace TouristarBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FlightAlertController : Controller
{
    private readonly IFlightAlertService _service;

    public FlightAlertController(IFlightAlertService service)
    {
        _service = service;
    }

    [HttpPost]
    public ActionResult<bool> ReceiveAlert(object body)
    {
        return Ok(true);
    }
}
