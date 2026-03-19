using AppCore.Interfaces;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryContactUnitOfWork(
    IPersonRepository persons,
    ICompanyRepository companies,
    IOrganizationRepository organizations
) : IContactUnitOfWork
{
    public IPersonRepository Persons => persons;
    public ICompanyRepository Companies => companies;
    public IOrganizationRepository Organizations => organizations;

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task<int> SaveChangesAsync()
    {
        return Task.FromResult(0);
    }

    public Task BeginTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync()
    {
        return Task.CompletedTask;
    }
}