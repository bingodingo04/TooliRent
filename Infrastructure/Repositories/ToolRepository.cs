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
    public class ToolRepository : Repository<Tool>, IToolRepository
    {
        public ToolRepository(TooliRentDbContext ctx) : base(ctx) { }

        public async Task<bool> IsToolAvailable(Guid toolId, DateTime start, DateTime end, CancellationToken ct)
        {
            return !await _ctx.BookingItems
                .Include(bi => bi.Booking)
                .Where(bi => bi.ToolId == toolId &&
                             bi.Booking.Status != BookingStatus.Cancelled &&
                             bi.Booking.Status != BookingStatus.Returned)
                .AnyAsync(bi => !(bi.Booking.EndAt <= start || bi.Booking.StartAt >= end), ct);
        }

        public async Task<IReadOnlyList<Tool>> GetFilteredAsync(Guid? categoryId, ToolStatus? status, bool? available,
            DateTime? from, DateTime? to, string? search, CancellationToken ct)
        {
            var q = _ctx.Tools.Include(t => t.Category).AsQueryable();

            if (categoryId is not null) q = q.Where(t => t.CategoryId == categoryId);
            if (status is not null) q = q.Where(t => t.Status == status);
            if (!string.IsNullOrWhiteSpace(search)) q = q.Where(t => t.Name.Contains(search));

            if (available == true && from is not null && to is not null)
            {
                var f = from.Value; var tt = to.Value;
                q = q.Where(t => !_ctx.BookingItems
                    .Include(bi => bi.Booking)
                    .Any(bi => bi.ToolId == t.Id &&
                               bi.Booking.Status != BookingStatus.Cancelled &&
                               bi.Booking.Status != BookingStatus.Returned &&
                               !(bi.Booking.EndAt <= f || bi.Booking.StartAt >= tt)));
            }

            return await q.AsNoTracking().ToListAsync(ct);
        }
    }
}
