using AppCore.Interfaces;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.Memory.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Memory;

public static class ContactsMemoryInfrastructureModule
{
    public static IServiceCollection AddContactsMemoryModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<MemoryDataStore>();

        services.AddSingleton<IPersonRepository, MemoryPersonRepository>();
        services.AddSingleton<ICompanyRepository, MemoryCompanyRepository>();
        services.AddSingleton<IOrganizationRepository, MemoryOrganizationRepository>();
        services.AddSingleton<IContactRepository, MemoryContactRepository>();

        services.AddSingleton<IContactUnitOfWork, MemoryContactUnitOfWork>();
        services.AddSingleton<IPersonService, PersonService>();

        return services;
    }
}