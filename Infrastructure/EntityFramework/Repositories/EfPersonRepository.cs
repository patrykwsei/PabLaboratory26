using AppCore.Dto;
using AppCore.Models.Contacts;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfPersonRepository(ContactsDbContext context)
    : EfGenericRepository<Person>(context.People), IPersonRepository
{
    public async Task<IEnumerable<Person>> FindByEmployerAsync(Guid companyId)
    {
        return await context.People
            .Include(p => p.Employer)
            .Include(p => p.Organization)
            .Include(p => p.Notes)
            .Include(p => p.Tags)
            .Where(p => p.Employer != null && p.Employer.Id == companyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> FindByOrganizationAsync(Guid organizationId)
    {
        return await context.People
            .Include(p => p.Employer)
            .Include(p => p.Organization)
            .Include(p => p.Notes)
            .Include(p => p.Tags)
            .Where(p => p.Organization != null && p.Organization.Id == organizationId)
            .ToListAsync();
    }

    public override async Task<Person?> FindByIdAsync(Guid id)
    {
        return await context.People
            .Include(p => p.Employer)
            .Include(p => p.Organization)
            .Include(p => p.Notes)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<Person>> FindAllAsync()
    {
        return await context.People
            .Include(p => p.Employer)
            .Include(p => p.Organization)
            .Include(p => p.Notes)
            .Include(p => p.Tags)
            .ToListAsync();
    }

    public override async Task<PagedResult<Person>> FindPagedAsync(int page, int pageSize)
    {
        var query = context.People
            .Include(p => p.Employer)
            .Include(p => p.Organization)
            .Include(p => p.Notes)
            .Include(p => p.Tags)
            .AsNoTracking();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await context.People.CountAsync();

        return new PagedResult<Person>(items, totalCount, page, pageSize);
    }
}