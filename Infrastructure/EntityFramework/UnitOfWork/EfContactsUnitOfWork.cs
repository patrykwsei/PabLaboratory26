using AppCore.Interfaces;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.EntityFramework.UnitOfWork;

public class EfContactsUnitOfWork(
    IPersonRepository personRepository,
    ICompanyRepository companyRepository,
    IOrganizationRepository organizationRepository,
    ContactsDbContext context) : IContactUnitOfWork
{
    public IPersonRepository Persons => personRepository;
    public ICompanyRepository Companies => companyRepository;
    public IOrganizationRepository Organizations => organizationRepository;

    public ValueTask DisposeAsync()
    {
        return context.DisposeAsync();
    }

    public Task<int> SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }

    public Task BeginTransactionAsync()
    {
        return context.Database.BeginTransactionAsync();
    }

    public Task CommitTransactionAsync()
    {
        return context.Database.CommitTransactionAsync();
    }

    public Task RollbackTransactionAsync()
    {
        return context.Database.RollbackTransactionAsync();
    }
}