using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookings;
        private readonly IToolRepository _tools;
        private readonly IMapper _map;
        private readonly TooliRentDbContext _ctx;

        public BookingService(IBookingRepository bookings, IToolRepository tools, IMapper map, TooliRentDbContext ctx)
        { _bookings = bookings; _tools = tools; _map = map; _ctx = ctx; }

        public async Task<BookingReadDto> CreateAsync(Guid memberId, BookingCreateDto dto, CancellationToken ct)
        {
            // Validera tillgänglighet
            foreach (var toolId in dto.ToolIds.Distinct())
            {
                var ok = await _tools.IsToolAvailable(toolId, dto.StartAt, dto.EndAt, ct);
                if (!ok) throw new Exception($"Verktyg {toolId} är inte tillgängligt i vald period.");
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                MemberId = memberId,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                Status = BookingStatus.Confirmed,
                Items = dto.ToolIds.Distinct().Select(id => new BookingItem { ToolId = id }).ToList()
            };
            await _bookings.AddAsync(booking, ct);

            // Markera verktyg som reserverade (valfritt, men tydligt)
            var affected = await _ctx.Tools.Where(t => dto.ToolIds.Contains(t.Id)).ToListAsync(ct);
            foreach (var t in affected) if (t.Status == ToolStatus.Available) t.Status = ToolStatus.Reserved;
            await _ctx.SaveChangesAsync(ct);

            return _map.Map<BookingReadDto>(await _bookings.GetWithItemsAsync(booking.Id, ct)!);
        }

        public async Task<IReadOnlyList<BookingReadDto>> GetMineAsync(Guid memberId, CancellationToken ct)
        {
            var items = await _bookings.GetForUserAsync(memberId, ct);
            return items.Select(_map.Map<BookingReadDto>).ToList();
        }

        public async Task<bool> CancelAsync(Guid memberId, Guid bookingId, CancellationToken ct)
        {
            var b = await _bookings.GetWithItemsAsync(bookingId, ct);
            if (b is null || b.MemberId != memberId) return false;
            if (b.Status is BookingStatus.CheckedOut or BookingStatus.Returned) return false;
            b.Status = BookingStatus.Cancelled;
            await _bookings.UpdateAsync(b, ct);

            // Frigör verktyg
            var ids = b.Items.Select(i => i.ToolId).ToList();
            var tools = await _ctx.Tools.Where(t => ids.Contains(t.Id)).ToListAsync(ct);
            foreach (var t in tools)
                if (t.Status == ToolStatus.Reserved) t.Status = ToolStatus.Available;
            await _ctx.SaveChangesAsync(ct);

            return true;
        }

        public async Task<BookingReadDto?> PickupAsync(Guid actorId, Guid bookingId, bool isAdmin, CancellationToken ct)
        {
            var b = await _bookings.GetWithItemsAsync(bookingId, ct);
            if (b is null) return null;
            if (!isAdmin && b.MemberId != actorId) return null;

            if (b.Status != BookingStatus.Confirmed) throw new Exception("Bokningen är inte i ett hämtbart läge.");
            b.Status = BookingStatus.CheckedOut;
            b.PickedUpAt = DateTime.UtcNow;
            await _bookings.UpdateAsync(b, ct);

            var tools = await _ctx.Tools.Where(t => b.Items.Select(i => i.ToolId).Contains(t.Id)).ToListAsync(ct);
            foreach (var t in tools) t.Status = ToolStatus.CheckedOut;
            await _ctx.SaveChangesAsync(ct);

            return _map.Map<BookingReadDto>(b);
        }

        public async Task<BookingReadDto?> ReturnAsync(Guid actorId, Guid bookingId, bool isAdmin, CancellationToken ct)
        {
            var b = await _bookings.GetWithItemsAsync(bookingId, ct);
            if (b is null) return null;
            if (!isAdmin && b.MemberId != actorId) return null;

            if (b.Status != BookingStatus.CheckedOut) throw new Exception("Bokningen är inte utlånad.");
            b.Status = BookingStatus.Returned;
            b.ReturnedAt = DateTime.UtcNow;
            await _bookings.UpdateAsync(b, ct);

            var tools = await _ctx.Tools.Where(t => b.Items.Select(i => i.ToolId).Contains(t.Id)).ToListAsync(ct);
            foreach (var t in tools) t.Status = ToolStatus.Available;
            await _ctx.SaveChangesAsync(ct);

            // Hantering av försenade: sätt Overdue om EndAt < ReturnedAt (för statistik/avgift i framtiden)
            if (b.EndAt < b.ReturnedAt) { /* logga avgift / flagga */ }

            return _map.Map<BookingReadDto>(b);
        }
    }
}
