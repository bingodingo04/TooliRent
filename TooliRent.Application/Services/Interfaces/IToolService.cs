using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IToolService
    {
        Task<IReadOnlyList<ToolReadDto>> GetAsync(Guid? categoryId, ToolStatus? status, bool? available, DateTime? from, DateTime? to, string? search, CancellationToken ct);
        Task<ToolReadDto?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<ToolReadDto> CreateAsync(ToolCreateUpdateDto dto, CancellationToken ct);
        Task<ToolReadDto?> UpdateAsync(Guid id, ToolCreateUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    }
}
