using AppCore.Models.Contacts;
using AppCore.Repositories;
using Infrastructure.Memory;

namespace Infrastructure.Memory.Repositories;

public class MemoryCompanyRepository : MemoryGenericRepository<Company>, ICompanyRepository
{
    private readonly MemoryDataStore _store;

    public MemoryCompanyRepository(MemoryDataStore store) : base(store.Companies)
    {
        _store = store;
    }

    public override async Task<Company> AddAsync(Company entity)
    {
        var added = await base.AddAsync(entity);
        _store.Contacts[added.Id] = added;
        return added;
    }

    public override async Task<Company> UpdateAsync(Company entity)
    {
        var updated = await base.UpdateAsync(entity);
        _store.Contacts[updated.Id] = updated;
        return updated;
    }

    public override async Task RemoveByIdAsync(Guid id)
    {
        await base.RemoveByIdAsync(id);
        _store.Contacts.Remove(id);
    }

    public Task<IEnumerable<Company>> FindByNameAsync(string namePart)
    {
        var result = _data.Values.Where(c => c.Name.Contains(namePart, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(result.AsEnumerable());
    }

    public Task<Company?> FindByNipAsync(string nip)
    {
        var result = _data.Values.FirstOrDefault(c => c.NIP == nip);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Person>> GetEmployeesAsync(Guid companyId)
    {
        if (!_data.TryGetValue(companyId, out var company))
            throw new KeyNotFoundException($"Company with Id={companyId} not found.");

        return Task.FromResult(company.Employees.AsEnumerable());
    }
}