using EnterpriseIDP.Application.Common.Interfaces;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Infrastructure.Authentication;
using EnterpriseIDP.Infrastructure.Persistence;
using EnterpriseIDP.Infrastructure.Persistence.Repositories;
using EnterpriseIDP.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseIDP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IGitHubService, GitHubService>();
        services.AddSingleton<IKubernetesService, KubernetesService>();

        return services;
    }
}
