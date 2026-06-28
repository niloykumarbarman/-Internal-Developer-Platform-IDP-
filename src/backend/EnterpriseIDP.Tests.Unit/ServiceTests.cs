using EnterpriseIDP.Domain.Entities.Catalog;
using EnterpriseIDP.Domain.Enums;
using EnterpriseIDP.Domain.Events.Catalog;
using FluentAssertions;
using Xunit;

namespace EnterpriseIDP.Tests.Unit;

public class ServiceTests
{
    private static Service CreateValidService(out Guid teamId, out Guid userId)
    {
        teamId = Guid.NewGuid();
        userId = Guid.NewGuid();
        var result = Service.Create(
            name: "API Gateway",
            slug: "api-gateway",
            description: "Main gateway",
            type: ServiceType.Gateway,
            ownerTeamId: teamId,
            ownerUserId: userId,
            repositoryUrl: "https://github.com/org/api-gateway",
            createdBy: "tester");

        result.IsError.Should().BeFalse();
        return result.Value;
    }

    [Fact]
    public void Create_WithValidData_ReturnsServiceWithActiveStatus()
    {
        var service = CreateValidService(out var teamId, out var userId);

        service.Name.Should().Be("API Gateway");
        service.Slug.Value.Should().Be("api-gateway");
        service.Status.Should().Be(ServiceStatus.Active);
        service.OwnerTeamId.Should().Be(teamId);
        service.OwnerUserId.Should().Be(userId);
    }

    [Fact]
    public void Create_WithValidData_RaisesServiceRegisteredEvent()
    {
        var service = CreateValidService(out _, out _);

        var domainEvent = service.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ServiceRegisteredEvent>().Subject;

        domainEvent.ServiceId.Should().Be(service.Id);
        domainEvent.ServiceName.Should().Be(service.Name);
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsValidationError()
    {
        var result = Service.Create(
            name: "  ",
            slug: "api-gateway",
            description: null,
            type: ServiceType.Gateway,
            ownerTeamId: Guid.NewGuid(),
            ownerUserId: Guid.NewGuid(),
            repositoryUrl: null,
            createdBy: "tester");

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Service.Name");
    }

    [Fact]
    public void Create_WithInvalidSlug_PropagatesSlugError()
    {
        var result = Service.Create(
            name: "API Gateway",
            slug: "Invalid Slug!",
            description: null,
            type: ServiceType.Gateway,
            ownerTeamId: Guid.NewGuid(),
            ownerUserId: Guid.NewGuid(),
            repositoryUrl: null,
            createdBy: "tester");

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Slug.Invalid");
    }

    [Fact]
    public void AddDependency_OnItself_ReturnsValidationError()
    {
        var service = CreateValidService(out _, out _);

        var result = service.AddDependency(service.Id, "tester");

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Service.SelfDependency");
    }

    [Fact]
    public void AddDependency_Duplicate_ReturnsConflictError()
    {
        var service = CreateValidService(out _, out _);
        var dependsOnId = Guid.NewGuid();
        service.AddDependency(dependsOnId, "tester");

        var result = service.AddDependency(dependsOnId, "tester");

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Service.DependencyExists");
    }

    [Fact]
    public void AddDependency_Valid_AddsToList()
    {
        var service = CreateValidService(out _, out _);
        var dependsOnId = Guid.NewGuid();

        var result = service.AddDependency(dependsOnId, "tester");

        result.IsError.Should().BeFalse();
        service.Dependencies.Should().ContainSingle(d => d.DependsOnServiceId == dependsOnId);
    }
}
