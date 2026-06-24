using EnterpriseIDP.Domain.Entities.Auth;
using EnterpriseIDP.Domain.Entities.Catalog;
using EnterpriseIDP.Domain.Entities.CICD;
using EnterpriseIDP.Domain.Entities.GitOps;
using EnterpriseIDP.Domain.Entities.Kubernetes;
using EnterpriseIDP.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseIDP.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceDependency> ServiceDependencies => Set<ServiceDependency>();
    public DbSet<ServiceTag> ServiceTags => Set<ServiceTag>();

    public DbSet<Pipeline> Pipelines => Set<Pipeline>();
    public DbSet<PipelineRun> PipelineRuns => Set<PipelineRun>();

    public DbSet<Repository> Repositories => Set<Repository>();

    public DbSet<KubernetesNamespace> KubernetesNamespaces => Set<KubernetesNamespace>();
    public DbSet<KubernetesDeployment> KubernetesDeployments => Set<KubernetesDeployment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression GetSoftDeleteFilter(Type type)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(type, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(condition, parameter);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
