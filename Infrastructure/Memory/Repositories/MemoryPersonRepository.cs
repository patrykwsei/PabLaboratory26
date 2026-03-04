using AppCore.Models.Contacts;
using AppCore.Repositories;

namespace Infrastructure.Memory.Repositories;

public class MemoryPersonRepository : MemoryGenericRepository<Person>, IPersonRepository
{
    public Task<IEnumerable<Person>> FindByEmployerAsync(Guid companyId)
    {
        var result = _data.Values
            .Where(p => p.Employer is not null && p.Employer.Id == companyId)
            .AsEnumerable();

        return Task.FromResult(result);
    }

    public Task<IEnumerable<Person>> FindByOrganizationAsync(Guid organizationId)
    {
        var result = _data.Values
            .Where(p => p.Organization is not null && p.Organization.Id == organizationId)
            .AsEnumerable();

        return Task.FromResult(result);
    }
}