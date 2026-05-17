using System.Collections.Generic;

namespace CarparkManagementSystem.ViewModels.Home;

public class ITDashboardViewModel
{
    public string CurrentRole { get; init; } = "IT Technician";

    public int InactiveSpaces { get; init; }

    public int SitesNeedingLayoutReview { get; init; }

    public string SystemStatusMessage { get; init; } = string.Empty;

     // New layout health stats
    public int UnpositionedSpaces { get; set; }
    public int TotalSites { get; set; }
    public int HealthySites { get; set; }
    public int TotalSpaces { get; set; }

    // New: per-site health breakdown for the dashboard table
    public List<SiteHealthViewModel>        SiteHealthSummary { get; set; } = [];
    public List<SiteSummaryCardViewModel>   Sites             { get; set; } = [];
    public List<ManagerBookingRowViewModel>  RecentBookings    { get; set; } = [];
}

public class SiteHealthViewModel
{
    public int    SiteId             { get; set; }
    public string SiteName           { get; set; } = string.Empty;
    public int    TotalSpaces        { get; set; }
    public int    InactiveSpaces     { get; set; }
    public int    UnpositionedSpaces { get; set; }

    public bool   HasIssues    => InactiveSpaces > 0 || UnpositionedSpaces > 0;
    public string HealthClass  => HasIssues ? "status-badge--warning" : "status-badge--active";
    public string HealthLabel  => HasIssues ? "Needs review" : "Healthy";
}
