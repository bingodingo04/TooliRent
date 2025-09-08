using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _map;
        public CategoryService(ICategoryRepository repo, IMapper map) { _repo = repo; _map = map; }

        public async Task<IReadOnlyList<CategoryReadDto>> GetAllAsync(CancellationToken ct) =>
            (await _repo.GetAllAsync(ct)).Select(_map.Map<CategoryReadDto>).ToList();

        public async Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct)
        {
            var e = await _repo.AddAsync(new Category { Id = Guid.NewGuid(), Name = dto.Name }, ct);
            return _map.Map<CategoryReadDto>(e);
        }

        public async Task<bool> UpdateAsync(Guid id, CategoryUpdateDto dto, CancellationToken ct)
        {
            var e = await _repo.GetByIdAsync(id, ct);
            if (e is null) return false;
            e.Name = dto.Name;
            await _repo.UpdateAsync(e, ct);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        {
            var e = await _repo.GetByIdAsync(id, ct);
            if (e is null) return false;
            await _repo.DeleteAsync(e, ct);
            return true;
        }
    }
}
