using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Enums;
using EnterpriseIDP.Domain.ValueObjects;
using EnterpriseIDP.Domain.Events.Catalog;
using ErrorOr;

namespace EnterpriseIDP.Domain.Entities.Catalog;

public sealed class Service : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public ServiceSlug Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public ServiceType Type { get; private set; }
    public ServiceStatus Status { get; private set; }
    public string? RepositoryUrl { get; private set; }
    public string? DocumentationUrl { get; private set; }
    public string? ApiSpecUrl { get; private set; }
    public string? TechStack { get; private set; }
    public string? Version { get; private set; }
    public Guid OwnerTeamId { get; private set; }
    public Guid OwnerUserId { get; private set; }

    private readonly List<ServiceDependency> _dependencies = [];
    public IReadOnlyList<ServiceDependency> Dependencies => _dependencies.AsReadOnly();

    private readonly List<ServiceTag> _tags = [];
    public IReadOnlyList<ServiceTag> Tags => _tags.AsReadOnly();

    private Service() { }

    public static ErrorOr<Service> Create(
        string name,
        string slug,
        string? description,
        ServiceType type,
        Guid ownerTeamId,
        Guid ownerUserId,
        string? repositoryUrl,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Service.Name", "Service name cannot be empty.");

        var slugResult = ServiceSlug.Create(slug);
        if (slugResult.IsError) return slugResult.Errors;

        var service = new Service
        {
            Name = name.Trim(),
            Slug = slugResult.Value,
            Description = description,
            Type = type,
            Status = ServiceStatus.Active,
            OwnerTeamId = ownerTeamId,
            OwnerUserId = ownerUserId,
            RepositoryUrl = repositoryUrl,
            CreatedBy = createdBy
        };

        service.AddDomainEvent(new ServiceRegisteredEvent(service.Id, service.Name, service.Slug.Value, ownerTeamId));
        return service;
    }

    public ErrorOr<Success> AddDependency(Guid dependsOnServiceId, string addedBy)
    {
        if (_dependencies.Any(d => d.DependsOnServiceId == dependsOnServiceId))
            return Error.Conflict("Service.DependencyExists", "Dependency already exists.");

        if (dependsOnServiceId == Id)
            return Error.Validation("Service.SelfDependency", "Service cannot depend on itself.");

        _dependencies.Add(ServiceDependency.Create(Id, dependsOnServiceId, addedBy));
        SetUpdated(addedBy);
        return Result.Success;
    }

    public void AddTag(string key, string value, string addedBy)
    {
        var existing = _tags.FirstOrDefault(t => t.Key == key);
        if (existing is not null) _tags.Remove(existing);
        _tags.Add(ServiceTag.Create(Id, key, value, addedBy));
        SetUpdated(addedBy);
    }

    public void UpdateStatus(ServiceStatus status, string updatedBy)
    {
        Status = status;
        SetUpdated(updatedBy);
    }

    public void UpdateVersion(string version, string updatedBy)
    {
        Version = version;
        SetUpdated(updatedBy);
    }

    public void Update(string name, string? description, string? repositoryUrl, string? documentationUrl, string? techStack, string updatedBy)
    {
        Name = name.Trim();
        Description = description;
        RepositoryUrl = repositoryUrl;
        DocumentationUrl = documentationUrl;
        TechStack = techStack;
        SetUpdated(updatedBy);
    }
}
