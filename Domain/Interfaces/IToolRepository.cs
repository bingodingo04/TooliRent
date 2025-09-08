using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IToolRepository : IRepository<Tool>
    {
        Task<bool> IsToolAvailable(Guid toolId, DateTime start, DateTime end, CancellationToken ct);
        Task<IReadOnlyList<Tool>> GetFilteredAsync(Guid? categoryId, ToolStatus? status, bool? available, DateTime? from, DateTime? to, string? search, CancellationToken ct);
    }
}
