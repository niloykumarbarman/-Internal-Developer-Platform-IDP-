using EnterpriseIDP.Domain.Entities.Auth;
using EnterpriseIDP.Domain.Entities.Catalog;
using EnterpriseIDP.Domain.Entities.CICD;
using EnterpriseIDP.Domain.Entities.GitOps;
using EnterpriseIDP.Domain.Entities.Kubernetes;
using EnterpriseIDP.Domain.Entities;
using EnterpriseIDP.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseIDP.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Phase 1 — Core
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

    // Phase 3 — Incident Management
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<IncidentTimeline> IncidentTimelines => Set<IncidentTimeline>();
    public DbSet<Postmortem> Postmortems => Set<Postmortem>();

    // Phase 3 — Audit Logs
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Phase 3 — Cost Management
    public DbSet<CostReport> CostReports => Set<CostReport>();
    public DbSet<ServiceCost> ServiceCosts => Set<ServiceCost>();
    public DbSet<BudgetAlert> BudgetAlerts => Set<BudgetAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Incident
        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.AffectedService).HasMaxLength(100);
            entity.Property(e => e.AssignedTo).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Labels)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            entity.HasMany(e => e.Timeline)
                .WithOne(t => t.Incident)
                .HasForeignKey(t => t.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // IncidentTimeline
        modelBuilder.Entity<IncidentTimeline>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Author).HasMaxLength(100);
            entity.Property(e => e.EventType).HasMaxLength(50);
        });

        // Postmortem
        modelBuilder.Entity<Postmortem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Summary).HasMaxLength(2000);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.HasOne(e => e.Incident)
                .WithOne()
                .HasForeignKey<Postmortem>(e => e.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityType).HasMaxLength(100);
            entity.Property(e => e.UserId).HasMaxLength(100);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.UserId);
        });

        // CostReport
        modelBuilder.Entity<CostReport>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TeamName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Namespace).HasMaxLength(100);
            entity.Property(e => e.Period).HasMaxLength(50);
            entity.Property(e => e.TotalCost).HasPrecision(18, 4);
            entity.Property(e => e.CpuCost).HasPrecision(18, 4);
            entity.Property(e => e.MemoryCost).HasPrecision(18, 4);
            entity.Property(e => e.StorageCost).HasPrecision(18, 4);
            entity.Property(e => e.NetworkCost).HasPrecision(18, 4);
            entity.HasMany(e => e.ServiceCosts)
                .WithOne(s => s.CostReport)
                .HasForeignKey(s => s.CostReportId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.TeamName);
            entity.HasIndex(e => e.PeriodStart);
        });

        // ServiceCost
        modelBuilder.Entity<ServiceCost>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ServiceName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Namespace).HasMaxLength(100);
            entity.Property(e => e.TotalCost).HasPrecision(18, 4);
            entity.Property(e => e.CpuCost).HasPrecision(18, 4);
            entity.Property(e => e.MemoryCost).HasPrecision(18, 4);
        });

        // BudgetAlert
        modelBuilder.Entity<BudgetAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TeamName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Namespace).HasMaxLength(100);
            entity.Property(e => e.BudgetLimit).HasPrecision(18, 4);
            entity.Property(e => e.CurrentSpend).HasPrecision(18, 4);
            entity.Property(e => e.AlertThresholdPercent).HasPrecision(5, 2);
            entity.HasIndex(e => e.TeamName);
        });

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
        var condition = System.Linq.Expressions.Expression.Equal(property,
            System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(condition, parameter);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
