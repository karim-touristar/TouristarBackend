using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristarBackend.Helpers;
using TouristarModels.Models;
using TouristarBackend.Contracts;

namespace TouristarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TripInvitationController : ControllerBase
    {
        private readonly ITripInvitationService _service;

        public TripInvitationController(ITripInvitationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<TripInvitation>> Post(CreateTripInvitationDto data)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = await _service.CreateInvitation(userId, data.TripId, data.InvitedEmail);
            return Ok(result);
        }
    }
}
