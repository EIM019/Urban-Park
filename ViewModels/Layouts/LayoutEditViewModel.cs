using System.ComponentModel.DataAnnotations;
using CarparkManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarparkManagementSystem.ViewModels.Layouts;

public class LayoutEditViewModel
{
    public int SiteId { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public List<LayoutEditItemViewModel> Spaces { get; set; } = [];

    // For the add new space form — new
    [ValidateNever]
    public NewSpaceViewModel NewSpace { get; set; } = new();
}

public class LayoutEditItemViewModel
{
    public int ParkingSpaceId { get; set; }
    public string? CardBackgroundColor { get; set; }
    public string? CardTextColor { get; set; }
    public string? CardFontWeight { get; set; }

    [Required, StringLength(10)]
    [Display(Name = "Space number")]
    public string SpaceNumber { get; set; } = string.Empty;

    [Required, Display(Name = "Space type")]
    public ParkingSpaceType SpaceType { get; set; }

    [Range(0, 99), Display(Name = "Row")]
    public int Row { get; set; }

    [Range(0, 99), Display(Name = "Column")]
    public int Column { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }

    // For display context — new
    public bool HasActiveBooking { get; set; }
    public string? ActiveBookingInfo { get; set; }     // e.g. "Booked until 17:00"
    public List<SelectListItem> SpaceTypeOptions { get; set; } = [];
}

// New: allows IT to add a brand new space to a site
public class NewSpaceViewModel
{
    [Required, StringLength(10)]
    [Display(Name = "Space number")]
    public string SpaceNumber { get; set; } = string.Empty;

    [Required, Display(Name = "Space type")]
    public ParkingSpaceType SpaceType { get; set; }

    [Range(1, 99), Display(Name = "Row")]
    public int Row { get; set; } = 1;

    [Range(1, 99), Display(Name = "Column")]
    public int Column { get; set; } = 1;

    public List<SelectListItem> SpaceTypeOptions { get; set; } = [];
}