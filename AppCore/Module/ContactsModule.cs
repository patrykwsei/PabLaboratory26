using AppCore.Dto;
using AppCore.Validators;
using AppCore.Validators.Shared;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.Module;

public static class ContactsModule
{
    public static IServiceCollection AddContactsCoreModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IValidator<CreatePersonDto>, CreatePersonDtoValidator>();
        services.AddScoped<IValidator<UpdatePersonDto>, UpdatePersonDtoValidator>();
        services.AddScoped<IValidator<AddressDto>, AddressDtoValidator>();

        services.AddFluentValidationAutoValidation();

        return services;
    }
}