using AppCore.Models.Contacts;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfCompanyRepository(ContactsDbContext context)
    : EfGenericRepository<Company>(context.Companies), ICompanyRepository
{
    public async Task<IEnumerable<Person>> GetEmployeesAsync(Guid companyId)
    {
        return await context.People
            .Include(p => p.Employer)
            .Where(p => p.Employer != null && p.Employer.Id == companyId)
            .ToListAsync();
    }

    public async Task<Company?> FindByNipAsync(string nip)
    {
        return await context.Companies
            .FirstOrDefaultAsync(c => c.NIP == nip);
    }

    public async Task<IEnumerable<Company>> FindByNameAsync(string name)
    {
        return await context.Companies
            .Where(c => c.Name.Contains(name))
            .ToListAsync();
    }
}