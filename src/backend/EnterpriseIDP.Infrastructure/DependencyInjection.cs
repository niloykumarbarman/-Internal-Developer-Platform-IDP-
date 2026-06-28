using System.Text;
using EnterpriseIDP.Application.Common.Interfaces;
using EnterpriseIDP.Application.Interfaces;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Infrastructure.Authentication;
using EnterpriseIDP.Infrastructure.Persistence;
using EnterpriseIDP.Infrastructure.Persistence.Repositories;
using EnterpriseIDP.Infrastructure.Repositories;
using EnterpriseIDP.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EnterpriseIDP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Generic Repository + UnitOfWork
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // HTTP Context
        services.AddHttpContextAccessor();

        // Core Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IHttpContextAccessorWrapper, HttpContextAccessorWrapper>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // JWT Authentication
        var jwtSecret = configuration["Jwt:Secret"] ?? "this-is-a-development-only-secret-key-please-change-it-in-production";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "EnterpriseIDP";
        var jwtAudience = configuration["Jwt:Audience"] ?? "EnterpriseIDP";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };
        });

        services.AddAuthorization();

        // GitHub + Kubernetes
        services.AddScoped<IGitHubService, GitHubService>();
        services.AddSingleton<IKubernetesService, KubernetesService>();

        // Phase 3 — Vault
        services.AddHttpClient<IVaultService, VaultService>(client =>
        {
            var vaultAddress = configuration["Vault:Address"] ?? "http://vault:8200";
            client.BaseAddress = new Uri(vaultAddress);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Phase 3 — Audit
        services.AddScoped<IAuditService, AuditService>();

        // Phase 3 — Incident Repository
        services.AddScoped<IIncidentRepository, IncidentRepository>();

        return services;
    }
}
