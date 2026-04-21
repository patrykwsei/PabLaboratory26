using AppCore.Authorization;
using AppCore.Interfaces;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.UnitOfWork;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.EntityFramework;

public static class ContactsInfrastructureModule
{
    public static IServiceCollection AddContactsEfModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ContactsDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("CrmDb")));

        services.AddScoped<IPersonRepository, EfPersonRepository>();
        services.AddScoped<ICompanyRepository, EfCompanyRepository>();
        services.AddScoped<IOrganizationRepository, EfOrganizationRepository>();
        services.AddScoped<IContactRepository, EfContactRepository>();

        services.AddScoped<IContactUnitOfWork, EfContactsUnitOfWork>();

        services.AddIdentity<CrmUser, CrmRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<ContactsDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDataSeeder, IdentityDbSeeder>();

        return services;
    }

    public static IServiceCollection AddJwt(this IServiceCollection services, JwtSettings jwtOptions)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = jwtOptions.GetSymmetricKey(),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(CrmPolicies.AdminOnly.ToString(), policy =>
                policy.RequireRole(UserRole.Administrator.ToString()));

            options.AddPolicy(CrmPolicies.SalesAccess.ToString(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(),
                    UserRole.SalesManager.ToString(),
                    UserRole.Salesperson.ToString()));

            options.AddPolicy(CrmPolicies.SalesManagerAccess.ToString(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(),
                    UserRole.SalesManager.ToString()));

            options.AddPolicy(CrmPolicies.SupportAccess.ToString(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(),
                    UserRole.SupportAgent.ToString()));

            options.AddPolicy(CrmPolicies.ReadOnlyAccess.ToString(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(),
                    UserRole.SalesManager.ToString(),
                    UserRole.Salesperson.ToString(),
                    UserRole.SupportAgent.ToString(),
                    UserRole.ReadOnly.ToString()));

            options.AddPolicy(CrmPolicies.ActiveUser.ToString(), policy =>
                policy
                    .RequireAuthenticatedUser()
                    .RequireClaim("status", SystemUserStatus.Active.ToString()));

            options.AddPolicy(CrmPolicies.SalesDepartment.ToString(), policy =>
                policy.RequireClaim("department", "Sales"));

            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}