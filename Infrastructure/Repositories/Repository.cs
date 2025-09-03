using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TooliRentDbContext _ctx;
        public Repository(TooliRentDbContext ctx) => _ctx = ctx;

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await _ctx.Set<T>().FindAsync([id], ct);

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
            await _ctx.Set<T>().AsNoTracking().ToListAsync(ct);

        public async Task<IReadOnlyList<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
            await _ctx.Set<T>().Where(predicate).AsNoTracking().ToListAsync(ct);

        public async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            _ctx.Set<T>().Add(entity);
            await _ctx.SaveChangesAsync(ct);
            return entity;
        }

        public async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _ctx.Set<T>().Update(entity);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            _ctx.Set<T>().Remove(entity);
            await _ctx.SaveChangesAsync(ct);
        }
    }

}
