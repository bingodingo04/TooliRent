using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryReadDto>> GetAllAsync(CancellationToken ct);
        Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct);
        Task<bool> UpdateAsync(Guid id, CategoryUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    }
}
