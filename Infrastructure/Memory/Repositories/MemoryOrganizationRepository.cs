using AppCore.Models.Contacts;
using AppCore.Models.Contacts.Enums;
using AppCore.Repositories;

namespace Infrastructure.Memory.Repositories;

public class MemoryOrganizationRepository : MemoryGenericRepository<Organization>, IOrganizationRepository
{
    public Task<IEnumerable<Organization>> FindByTypeAsync(OrganizationType type)
    {
        var result = _data.Values.Where(o => o.Type == type).AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId)
    {
        if (!_data.TryGetValue(organizationId, out var org))
            throw new KeyNotFoundException($"Organization with Id={organizationId} not found.");

        return Task.FromResult(org.Members.AsEnumerable());
    }
}