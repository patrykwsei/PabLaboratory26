using AppCore.Models.Contacts;
using AppCore.Repositories;
using Infrastructure.Memory;

namespace Infrastructure.Memory.Repositories;

public class MemoryPersonRepository : MemoryGenericRepository<Person>, IPersonRepository
{
    private readonly MemoryDataStore _store;

    public MemoryPersonRepository(MemoryDataStore store) : base(store.Persons)
    {
        _store = store;
    }

    public override async Task<Person> AddAsync(Person entity)
    {
        var added = await base.AddAsync(entity);
        _store.Contacts[added.Id] = added;
        return added;
    }

    public override async Task<Person> UpdateAsync(Person entity)
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

    public Task<IEnumerable<Person>> FindByEmployerAsync(Guid companyId)
    {
        var result = _data.Values.Where(p => p.Employer != null && p.Employer.Id == companyId);
        return Task.FromResult(result.AsEnumerable());
    }

    public Task<IEnumerable<Person>> FindByOrganizationAsync(Guid organizationId)
    {
        var result = _data.Values.Where(p => p.Organization != null && p.Organization.Id == organizationId);
        return Task.FromResult(result.AsEnumerable());
    }
}