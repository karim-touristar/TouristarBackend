using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Helpers;

namespace TouristarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokensResponseDto>> Register(UserRequestDto request)
        {
            var result = await _authService.Register(request);
            return Ok(result);
        }

        [HttpPost("login")]
        public ActionResult<TokensResponseDto> Login(UserLoginDto request)
        {
            var result = _authService.Login(request);
            return Ok(result);
        }

        [HttpPost("deviceToken")]
        [Authorize]
        public async Task<ActionResult<bool>> RegisterDeviceToken(RegisterDeviceTokenDto request)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            await _authService.RegisterDeviceToken(userId, request.deviceToken);
            return Ok(true);
        }

        [HttpGet("user")]
        [Authorize]
        public ActionResult<User> GetUser()
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = _authService.GetUser(userId);
            return Ok(result);
        }

        [HttpPut("user")]
        [Authorize]
        public async Task<ActionResult<User>> UpdateUser(UpdateUserRequestDto request)
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = await _authService.UpdateUser(userId, request);
            return Ok(result);
        }

        [HttpPost("deleteAccount")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteAccount()
        {
            var userId = AuthenticationHelper.GetUserId(HttpContext);
            var result = await _authService.DeleteAccount(userId);
            return Ok(result);
        }
    }
}
