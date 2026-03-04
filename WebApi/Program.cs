using AppCore;
using AppCore.Models.Contacts;
using AppCore.Repositories;
using Infrastructure.Memory.Repositories;


namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<IContactRepository, MemoryContactRepository>();
        builder.Services.AddSingleton<IPersonRepository, MemoryPersonRepository>();
        builder.Services.AddSingleton<ICompanyRepository, MemoryCompanyRepository>();
        builder.Services.AddSingleton<IOrganizationRepository, MemoryOrganizationRepository>();
        builder.Services.AddOpenApi();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapGet("/", () =>
        {
            var person = new Person
            {
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan@test.pl"
            };

            return person.GetDisplayName();
        });
           
        app.Run();
    }
}