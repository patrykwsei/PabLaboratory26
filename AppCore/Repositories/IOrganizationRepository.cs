using AppCore.Models.Contacts;
using AppCore.Models.Contacts.Enums;

namespace AppCore.Repositories;

public interface IOrganizationRepository : IGenericRepositoryAsync<Organization>
{
    Task<IEnumerable<Organization>> FindByTypeAsync(OrganizationType type);
    Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId);
}