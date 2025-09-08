using System.Security.Claims;
using Application.Services.Interfaces;
using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TooliRent.Api.Controllers
{

    [ApiController]
    [Route("api/bookings")]
    [Authorize(Policy = "MemberOnly")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _svc;
        public BookingsController(IBookingService svc) => _svc = svc;

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private bool IsAdmin => User.IsInRole("Admin");

        [HttpGet("me")]
        public async Task<ActionResult<IReadOnlyList<BookingReadDto>>> Mine(CancellationToken ct) =>
            Ok(await _svc.GetMineAsync(CurrentUserId, ct));

        [HttpPost]
        public async Task<ActionResult<BookingReadDto>> Create(BookingCreateDto dto, CancellationToken ct) =>
            Ok(await _svc.CreateAsync(CurrentUserId, dto, ct));

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken ct) =>
            await _svc.CancelAsync(CurrentUserId, id, ct) ? NoContent() : NotFound();

        [HttpPost("{id:guid}/pickup")]
        public async Task<ActionResult<BookingReadDto>> Pickup(Guid id, CancellationToken ct) =>
            Ok(await _svc.PickupAsync(CurrentUserId, id, IsAdmin, ct));

        [HttpPost("{id:guid}/return")]
        public async Task<ActionResult<BookingReadDto>> Return(Guid id, CancellationToken ct) =>
            Ok(await _svc.ReturnAsync(CurrentUserId, id, IsAdmin, ct));
    }
}