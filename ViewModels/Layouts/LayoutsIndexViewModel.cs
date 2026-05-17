namespace CarparkManagementSystem.ViewModels.Layouts;

public class LayoutsIndexViewModel
{
    public bool CanEditLayouts { get; init; }
    public List<LayoutSiteCardViewModel> Sites { get; init; } = [];

    // Summary stats across all sites — new
    public int TotalSites => Sites.Count;
    public int SitesWithIssues => Sites.Count(s => s.HasIssues);
    public int TotalInactiveSpaces => Sites.Sum(s => s.InactiveSpaces);
    public int TotalUnpositionedSpaces => Sites.Sum(s => s.UnpositionedSpaces);
}

public class LayoutSiteCardViewModel
{
    public int SiteId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int TotalSpaces { get; init; }
    public int StandardSpaces { get; init; }
    public int ElectricChargingSpaces { get; init; }
    public int DisabledSpaces { get; init; }
    public int VisitorSpaces { get; init; }

    // Health indicators — new
    public int InactiveSpaces { get; init; }
    public int UnpositionedSpaces { get; init; }
    public bool HasIssues => InactiveSpaces > 0 || UnpositionedSpaces > 0;
    public string HealthStatus => HasIssues ? "Needs review" : "Healthy";
    public string HealthClass => HasIssues ? "status-badge--warning" : "status-badge--active";
}