using AppCore.Module;
using Infrastructure.EntityFramework;
using Infrastructure.Security;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<JwtSettings>();
builder.Services.AddJwt(new JwtSettings(builder.Configuration));

builder.Services.AddContactsCoreModule(builder.Configuration);
builder.Services.AddContactsEfModule(builder.Configuration);

builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();