using AppCore.Dto;
using AppCore.Models.Common;
using AppCore.Repositories;

namespace Infrastructure.Memory.Repositories;

public class MemoryGenericRepository<T> : IGenericRepositoryAsync<T>
    where T : EntityBase
{
    protected readonly Dictionary<Guid, T> _data = new();

    public Task<T?> FindByIdAsync(Guid id)
    {
        var result = _data.TryGetValue(id, out var value) ? value : null;
        return Task.FromResult(result);
    }

    public Task<IEnumerable<T>> FindAllAsync()
    {
        IEnumerable<T> result = _data.Values.ToList();
        return Task.FromResult(result);
    }

    public Task<PagedResult<T>> FindPagedAsync(int page, int pageSize)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var totalCount = _data.Count;

        var items = _data.Values
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PagedResult<T>(items, totalCount, page, pageSize));
    }

    public Task<T> AddAsync(T entity)
    {
        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        if (_data.ContainsKey(entity.Id))
            throw new InvalidOperationException(
                $"{typeof(T).Name} with Id={entity.Id} already exists.");

        _data[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task<T> UpdateAsync(T entity)
    {
        if (entity.Id == Guid.Empty)
            throw new ArgumentException("Cannot update entity with empty Guid Id.");

        if (!_data.ContainsKey(entity.Id))
            throw new KeyNotFoundException(
                $"{typeof(T).Name} with Id={entity.Id} not found.");

        _data[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task RemoveByIdAsync(Guid id)
    {
        if (!_data.Remove(id))
            throw new KeyNotFoundException(
                $"{typeof(T).Name} with Id={id} not found.");

        return Task.CompletedTask;
    }
}