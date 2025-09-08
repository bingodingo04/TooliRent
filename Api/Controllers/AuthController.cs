using Application.Services.Interfaces;
using Application;
using Microsoft.AspNetCore.Mvc;

namespace TooliRent.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _svc;
        public AuthController(IAuthService svc) => _svc = svc;

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto, CancellationToken ct) =>
            Ok(await _svc.RegisterAsync(dto, ct));

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto, CancellationToken ct) =>
            Ok(await _svc.LoginAsync(dto, ct));

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] string refreshToken, CancellationToken ct) =>
            Ok(await _svc.RefreshAsync(refreshToken, ct));
    }
}