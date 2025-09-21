using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain;
using Infrastructure;
using Application.Services.Interfaces;



namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userMgr;
        private readonly SignInManager<AppUser> _signInMgr;
        private readonly IConfiguration _cfg;

        public AuthService(UserManager<AppUser> um, SignInManager<AppUser> sm, IConfiguration cfg)
        {
            _userMgr = um; _signInMgr = sm; _cfg = cfg;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct)
        {
            var user = new AppUser { Id = Guid.NewGuid(), Email = dto.Email, UserName = dto.Email, EmailConfirmed = true };
            var res = await _userMgr.CreateAsync(user, dto.Password);
            if (!res.Succeeded) throw new Exception(string.Join("; ", res.Errors.Select(e => e.Description)));
            await _userMgr.AddToRoleAsync(user, "Member");
            return await GenerateTokensAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct)
        {
            var user = await _userMgr.Users.FirstOrDefaultAsync(u => u.Email == dto.Email, ct)
                       ?? throw new Exception("Invalid credentials");
            if (!user.IsActive) throw new Exception("User inactive");
            var res = await _signInMgr.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!res.Succeeded) throw new Exception("Invalid credentials");
            return await GenerateTokensAsync(user);
        }

        public async Task<AuthResponseDto> RefreshAsync(RefreshRequest req, CancellationToken ct)
        {
            // Kursens “dummy”-regel
            if (req.RefreshToken != "dummy")
                throw new Exception("Invalid refresh token");

            // Hämta användaren som ska få ny access-token
            var user = await _userMgr.FindByIdAsync(req.UserId.ToString())
                       ?? throw new Exception("User not found");

            if (!user.IsActive) throw new Exception("User inactive");

            // Återanvänd din befintliga token-generator
            return await GenerateTokensAsync(user);
        }

        private async Task<AuthResponseDto> GenerateTokensAsync(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var roles = await _userMgr.GetRolesAsync(user);

            var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Email ?? ""),
        };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var expires = DateTime.UtcNow.AddHours(3);
            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new AuthResponseDto(new JwtSecurityTokenHandler().WriteToken(token), expires, "dummy", user.Id);
        }
    }
}
