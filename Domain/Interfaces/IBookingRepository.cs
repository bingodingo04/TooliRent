using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<Booking?> GetWithItemsAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<Booking>> GetForUserAsync(Guid userId, CancellationToken ct);
    }
}
