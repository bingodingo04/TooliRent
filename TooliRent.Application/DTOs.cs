using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public record RegisterDto(string Email, string Password);
    public record LoginDto(string Email, string Password);
    public record AuthResponseDto(string AccessToken, DateTime ExpiresAt, string RefreshToken);

    public record CategoryReadDto(Guid Id, string Name);
    public record CategoryCreateDto(string Name);
    public record CategoryUpdateDto(string Name);

    public record ToolReadDto(Guid Id, string Name, string CategoryName, ToolStatus Status, string? SerialNumber, string? Condition, string? Description);
    public record ToolCreateUpdateDto(string Name, Guid CategoryId, ToolStatus Status, string? SerialNumber, string? Condition, string? Description);

    public record BookingItemReadDto(Guid ToolId, string ToolName);
    public record BookingReadDto(Guid Id, DateTime StartAt, DateTime EndAt, BookingStatus Status,
        DateTime? PickedUpAt, DateTime? ReturnedAt, IReadOnlyList<BookingItemReadDto> Items);

    public record BookingCreateDto(DateTime StartAt, DateTime EndAt, IReadOnlyList<Guid> ToolIds);
}
