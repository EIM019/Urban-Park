using CarparkManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarparkManagementSystem.ViewModels.Bookings;

public class BookingsListViewModel
{
    public int? SiteId { get; set; }

    public string StatusFilter { get; set; } = "Active";

    public bool CanManageBookings { get; init; }

    public List<SelectListItem> SiteOptions { get; set; } = [];

    public List<SelectListItem> StatusOptions { get; set; } = [];

    public List<BookingListItemViewModel> Bookings { get; set; } = [];
}

public class BookingListItemViewModel
{
    public int Id { get; init; }

    public string SiteName { get; init; } = string.Empty;

    public string SpaceNumber { get; init; } = string.Empty;

    public ParkingSpaceType SpaceType { get; init; }

    public string RequesterName { get; init; } = string.Empty;

    public BookingPartyType PartyType { get; init; }

    public string? EmployeeNumber { get; init; }

    public string? Company { get; init; }

    public string ContactNumber { get; init; } = string.Empty;

    public DateTime StartDateTime { get; init; }

    public DateTime EndDateTime { get; init; }

    public bool IsPriority { get; init; }

    public BookingStatus Status { get; init; }
}
