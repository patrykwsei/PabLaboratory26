using AppCore.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.Module;

public static class ContactsModule
{
    public static IServiceCollection AddContactsModule(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreatePersonDtoValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }
}