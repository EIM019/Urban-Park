using CarparkManagementSystem.Models;

namespace CarparkManagementSystem.ViewModels.Layouts;

public class SiteLayoutViewModel
{
    public int SiteId { get; init; }
    public string SiteName { get; init; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int GridColumns { get; init; }
    public bool CanEdit { get; init; }

    // Space counts
    public int TotalSpaces { get; init; }
    public int AvailableSpaces { get; init; }
    public int BookedSpaces { get; init; }
    public int PriorityBookedSpaces { get; init; }

    // Layout health — new
    public int InactiveSpaces { get; init; }
    public int UnpositionedSpaces { get; init; }      // spaces with no Row/Column assigned
    public bool HasLayoutIssues => InactiveSpaces > 0 || UnpositionedSpaces > 0;

    // Space type breakdown — new
    public int StandardSpaces { get; init; }
    public int ElectricChargingSpaces { get; init; }
    public int DisabledSpaces { get; init; }
    public int VisitorSpaces { get; init; }

    public List<LayoutSpaceViewModel> Spaces { get; init; } = [];
}

public class LayoutSpaceViewModel
{
    public int ParkingSpaceId { get; init; }
    public string SpaceNumber { get; init; } = string.Empty;
    public ParkingSpaceType SpaceType { get; init; }
    public int Row { get; init; }
    public int Column { get; init; }
    public bool IsActive { get; init; }
    public bool IsAvailable { get; init; }
    public bool IsPriorityBooking { get; init; }
    public bool IsUnpositioned { get; init; }          // ← new: flags spaces missing grid position
    public string BookingLabel { get; init; } = string.Empty;
    public string? BookingRequester { get; init; }     // ← new: who booked it
    public DateTime? BookingEnd { get; init; }         // ← new: when it frees up
    public string? InactiveNote { get; init; }
    public bool IsInactivePermanent { get; init; }
    public DateTime? InactiveSince { get; init; }
    public string? CardBackgroundColor { get; set; }
    public string? CardTextColor { get; set; }
    public string? CardFontWeight { get; set; }
}