using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Helpers;
using TouristarBackend.Models;
using TouristarBackend.Constants;

namespace TouristarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _service;

        public InvitationController(IInvitationService service)
        {
            _service = service;
        }

        [HttpGet("{tripId}")]
        [Authorize]
        public async Task<ActionResult<InvitationUrlResponseDto>> Get(long tripId)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = await _service.CreateOrRetrieveItineraryLink(userId, tripId);
            return Ok(result);
        }

        [HttpGet("trip/{token}")]
        public ActionResult<PublicInvitationDto> GetTripFromToken(string token)
        {
            var result = _service.GetTripFromInvitation(token);
            return Ok(result);
        }
    }
}
