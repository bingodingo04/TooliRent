using Domain.Interfaces;
using Domain;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(TooliRentDbContext ctx) : base(ctx) { }

        public async Task<Booking?> GetWithItemsAsync(Guid id, CancellationToken ct) =>
            await _ctx.Bookings.Include(b => b.Items).ThenInclude(i => i.Tool)
                               .FirstOrDefaultAsync(b => b.Id == id, ct);

        public async Task<IReadOnlyList<Booking>> GetForUserAsync(Guid userId, CancellationToken ct) =>
            await _ctx.Bookings.Where(b => b.MemberId == userId)
                               .Include(b => b.Items).ThenInclude(i => i.Tool)
                               .OrderByDescending(b => b.StartAt)
                               .ToListAsync(ct);
    }
}
