using AppCore.Models.Contacts;

namespace AppCore.Repositories;

public interface IPersonRepository : IGenericRepositoryAsync<Person>
{
    Task<IEnumerable<Person>> FindByEmployerAsync(Guid companyId);
    Task<IEnumerable<Person>> FindByOrganizationAsync(Guid organizationId);
}