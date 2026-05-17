using CarparkManagementSystem.Models;

namespace CarparkManagementSystem.ViewModels.Home;

public class SiteSummaryCardViewModel
{
    public int SiteId { get; init; }

    public string Name { get; init; } = string.Empty;

    public int TotalSpaces { get; init; }

    public int StandardSpaces { get; init; }

    public int ElectricChargingSpaces { get; init; }

    public int DisabledSpaces { get; init; }

    public int VisitorSpaces { get; init; }

    public int AvailableInWindow { get; init; }
}

public class DashboardBookingViewModel
{
    public int Id { get; init; }

    public string SiteName { get; init; } = string.Empty;

    public string SpaceNumber { get; init; } = string.Empty;

    public string RequesterName { get; init; } = string.Empty;

    public BookingPartyType PartyType { get; init; }

    public DateTime StartDateTime { get; init; }

    public DateTime EndDateTime { get; init; }

    public bool IsPriority { get; init; }

    public BookingStatus Status { get; init; }

    public string? OverriddenRequesterName { get; set; }   // who was displaced
    public bool    IsOverrideReplacement   { get; set; } // true if this booking is a priority replacement for an overridden booking
}
