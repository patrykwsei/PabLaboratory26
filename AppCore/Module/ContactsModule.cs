using AppCore.Dto;
using AppCore.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.Module;

public static class ContactsModule
{
    public static IServiceCollection AddContactsModule(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreatePersonDto>, CreatePersonDtoValidator>();
        services.AddScoped<IValidator<UpdatePersonDto>, UpdatePersonDtoValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }
}