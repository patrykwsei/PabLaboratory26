using AppCore.Interfaces;
using AppCore.Module;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.Memory;
using Infrastructure.Memory.Repositories;
using Infrastructure.Services;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddContactsModule();

builder.Services.AddSingleton<MemoryDataStore>();

builder.Services.AddSingleton<IPersonRepository, MemoryPersonRepository>();
builder.Services.AddSingleton<ICompanyRepository, MemoryCompanyRepository>();
builder.Services.AddSingleton<IOrganizationRepository, MemoryOrganizationRepository>();
builder.Services.AddSingleton<IContactRepository, MemoryContactRepository>();

builder.Services.AddSingleton<IContactUnitOfWork, MemoryContactUnitOfWork>();
builder.Services.AddSingleton<IPersonService, MemoryPersonService>();

builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseExceptionHandler();
app.MapControllers();

app.Run();