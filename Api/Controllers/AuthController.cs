using Application.Services.Interfaces;
using Application;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace TooliRent.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _svc;
        public AuthController(IAuthService svc) => _svc = svc;

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto, IValidator<RegisterDto> validator, CancellationToken ct)
        {
            var result = await validator.ValidateAsync(dto, ct);
            if (!result.IsValid)
                return BadRequest(result.Errors);
            return Ok(await _svc.RegisterAsync(dto, ct));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto, CancellationToken ct) =>
            Ok(await _svc.LoginAsync(dto, ct));

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshRequest req, CancellationToken ct) =>
        Ok(await _svc.RefreshAsync(req, ct));
    }
}