using AppCore.Interfaces;
using Infrastructure.Memory;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<ICustomerService, MemoryCustomerService>();

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
        
        app.MapGet("/api/customers", (ICustomerService service,HttpContext httpContext) =>
            {
                return service.GetCustomers();
            })
            .WithName("GetCustomers");

        app.Run();
    }
}