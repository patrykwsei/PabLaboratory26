using AppCore.Models.Contacts;
using AppCore.Models.Contacts.Enums;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfOrganizationRepository(ContactsDbContext context)
    : EfGenericRepository<Organization>(context.Organizations), IOrganizationRepository
{
    public async Task<IEnumerable<Organization>> FindByTypeAsync(OrganizationType type)
    {
        return await context.Organizations
            .Where(o => o.Type == type)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId)
    {
        return await context.People
            .Include(p => p.Organization)
            .Where(p => p.Organization != null && p.Organization.Id == organizationId)
            .ToListAsync();
    }
}