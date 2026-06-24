namespace EnterpriseIDP.Domain.Entities;

public class CostReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TeamName { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalCost { get; set; }
    public decimal CpuCost { get; set; }
    public decimal MemoryCost { get; set; }
    public decimal StorageCost { get; set; }
    public decimal NetworkCost { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public List<ServiceCost> ServiceCosts { get; set; } = new();
}

public class ServiceCost
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CostReportId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
    public decimal CpuCost { get; set; }
    public decimal MemoryCost { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public int ReplicaCount { get; set; }
    public CostReport? CostReport { get; set; }
}

public class BudgetAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TeamName { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public decimal BudgetLimit { get; set; }
    public decimal CurrentSpend { get; set; }
    public decimal AlertThresholdPercent { get; set; } = 80;
    public bool IsActive { get; set; } = true;
    public bool IsTriggered { get; set; } = false;
    public DateTime? TriggeredAt { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
