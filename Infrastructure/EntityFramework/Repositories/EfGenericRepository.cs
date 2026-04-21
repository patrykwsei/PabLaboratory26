using AppCore.Dto;
using AppCore.Models.Common;
using AppCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfGenericRepository<T>(DbSet<T> set) : IGenericRepositoryAsync<T>
    where T : EntityBase
{
    public virtual async Task<T?> FindByIdAsync(Guid id)
    {
        return await set.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> FindAllAsync()
    {
        return await set.ToListAsync();
    }

    public virtual async Task<PagedResult<T>> FindPagedAsync(int page, int pageSize)
    {
        var items = await set
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>(
            items,
            await set.CountAsync(),
            page,
            pageSize
        );
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        var entry = await set.AddAsync(entity);
        return entry.Entity;
    }

    public virtual Task<T> UpdateAsync(T entity)
    {
        var entry = set.Update(entity);
        return Task.FromResult(entry.Entity);
    }

    public virtual Task RemoveByIdAsync(Guid id)
    {
        var entity = set.Find(id);
        if (entity is not null)
        {
            set.Remove(entity);
        }

        return Task.CompletedTask;
    }
}