using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristarBackend.Contracts;
using TouristarBackend.Helpers;
using TouristarModels.Models;

namespace TouristarBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StatsController : Controller
{
    private readonly IStatsService _service;

    public StatsController(IStatsService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<UserStatsDto> GetUserStats()
    {
        var userId = AuthenticationHelper.GetUserId(HttpContext);
        var result = _service.GetUserStats(userId);
        return Ok(result);
    }
}
