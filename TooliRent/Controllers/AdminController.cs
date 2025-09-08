using Domain;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TooliRent.Api.Controllers
{

    [ApiController]
    [Route("api/admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _um;
        private readonly TooliRentDbContext _ctx;
        public AdminController(UserManager<AppUser> um, TooliRentDbContext ctx) { _um = um; _ctx = ctx; }

        [HttpPost("users/{id:guid}/activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            var u = await _um.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (u is null) return NotFound();
            u.IsActive = true; await _um.UpdateAsync(u);
            return NoContent();
        }

        [HttpPost("users/{id:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var u = await _um.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (u is null) return NotFound();
            u.IsActive = false; await _um.UpdateAsync(u);
            return NoContent();
        }

        [HttpGet("stats")]
        public async Task<IActionResult> Stats()
        {
            var totalUsers = await _um.Users.CountAsync();
            var totalTools = await _ctx.Tools.CountAsync();
            var activeLoans = await _ctx.Bookings.CountAsync(b => b.Status == BookingStatus.CheckedOut);
            var topCats = await _ctx.Categories
                .Select(c => new { c.Name, Count = c.Tools.Count })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            return Ok(new { totalUsers, totalTools, activeLoans, topCategories = topCats });
        }
    }
}