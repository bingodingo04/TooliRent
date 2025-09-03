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
    public class ToolService : IToolService
    {
        private readonly IToolRepository _repo;
        private readonly IMapper _map;

        public ToolService(IToolRepository repo, IMapper map) { _repo = repo; _map = map; }

        public async Task<IReadOnlyList<ToolReadDto>> GetAsync(Guid? categoryId, ToolStatus? status, bool? available, DateTime? from, DateTime? to, string? search, CancellationToken ct)
        {
            var items = await _repo.GetFilteredAsync(categoryId, status, available, from, to, search, ct);
            return items.Select(_map.Map<ToolReadDto>).ToList();
        }

        public async Task<ToolReadDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var item = await _repo.GetByIdAsync(id, ct);
            if (item is null) return null;
            return _map.Map<ToolReadDto>(item);
        }

        public async Task<ToolReadDto> CreateAsync(ToolCreateUpdateDto dto, CancellationToken ct)
        {
            var entity = _map.Map<Tool>(dto);
            entity.Id = Guid.NewGuid();
            entity = await _repo.AddAsync(entity, ct);
            return _map.Map<ToolReadDto>(entity);
        }

        public async Task<ToolReadDto?> UpdateAsync(Guid id, ToolCreateUpdateDto dto, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null) return null;
            _map.Map(dto, entity);
            await _repo.UpdateAsync(entity, ct);
            return _map.Map<ToolReadDto>(entity);
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
