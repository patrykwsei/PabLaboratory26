using AppCore.Interfaces;
using AppCore.Module;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.Memory.Repositories;
using Infrastructure.Memory;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// rejestracja walidatorów (FluentValidation)
builder.Services.AddContactsModule();

builder.Services.AddSingleton<MemoryDataStore>();
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

app.Run();