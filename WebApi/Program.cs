using AppCore.Interfaces;
using AppCore.Repositories;
using AppCore.Services;
using AppCore.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Memory;
using Infrastructure.Memory.Repositories;
using Infrastructure.Services;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();

        builder.Services.AddControllers();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationClientsideAdapters();

        builder.Services.AddScoped<IValidator<AppCore.Dto.CreatePersonDto>, CreatePersonDtoValidator>();
        builder.Services.AddScoped<IValidator<AppCore.Dto.UpdatePersonDto>, UpdatePersonDtoValidator>();

        builder.Services.AddOpenApi();

        builder.Services.AddSingleton<MemoryDataStore>();

        builder.Services.AddSingleton<IContactRepository, MemoryContactRepository>();
        builder.Services.AddSingleton<IPersonRepository, MemoryPersonRepository>();
        builder.Services.AddSingleton<ICompanyRepository, MemoryCompanyRepository>();
        builder.Services.AddSingleton<IOrganizationRepository, MemoryOrganizationRepository>();

        builder.Services.AddSingleton<IContactUnitOfWork, MemoryContactUnitOfWork>();
        builder.Services.AddSingleton<IPersonService, MemoryPersonService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.MapGet("/", () => "api dziala");

        app.Run();
    }
}