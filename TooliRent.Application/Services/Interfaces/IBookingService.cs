using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingReadDto> CreateAsync(Guid memberId, BookingCreateDto dto, CancellationToken ct);
        Task<IReadOnlyList<BookingReadDto>> GetMineAsync(Guid memberId, CancellationToken ct);
        Task<bool> CancelAsync(Guid memberId, Guid bookingId, CancellationToken ct);
        Task<BookingReadDto?> PickupAsync(Guid actorId, Guid bookingId, bool isAdmin, CancellationToken ct);
        Task<BookingReadDto?> ReturnAsync(Guid actorId, Guid bookingId, bool isAdmin, CancellationToken ct);
    }
}
