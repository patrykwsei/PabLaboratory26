using AppCore.Models.Contacts;
using AppCore.Models.Contacts.Enums;
using AppCore.Repositories;
using Infrastructure.Memory;

namespace Infrastructure.Memory.Repositories;

public class MemoryOrganizationRepository : MemoryGenericRepository<Organization>, IOrganizationRepository
{
    private readonly MemoryDataStore _store;

    public MemoryOrganizationRepository(MemoryDataStore store) : base(store.Organizations)
    {
        _store = store;
    }

    public override async Task<Organization> AddAsync(Organization entity)
    {
        var added = await base.AddAsync(entity);
        _store.Contacts[added.Id] = added;
        return added;
    }

    public override async Task<Organization> UpdateAsync(Organization entity)
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

    public Task<IEnumerable<Organization>> FindByTypeAsync(OrganizationType type)
    {
        var result = _data.Values.Where(o => o.Type == type);
        return Task.FromResult(result.AsEnumerable());
    }

    public Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId)
    {
        if (!_data.TryGetValue(organizationId, out var organization))
            throw new KeyNotFoundException($"Organization with Id={organizationId} not found.");

        return Task.FromResult(organization.Members.AsEnumerable());
    }
}