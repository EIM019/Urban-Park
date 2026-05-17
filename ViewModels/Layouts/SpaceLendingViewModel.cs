using System.ComponentModel.DataAnnotations;
using CarparkManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarparkManagementSystem.ViewModels.Layouts;

public class SpaceLendingFormViewModel
{
    public int SiteId { get; set; }
    public string SiteName { get; set; } = string.Empty;

    // Selected space IDs to lend
    [Required]
    public List<int> SelectedSpaceIds { get; set; } = [];

    [Required, Display(Name = "Lend to space type")]
    public ParkingSpaceType LendToType { get; set; }

    [Required, StringLength(500), Display(Name = "Reason for lending")]
    public string Reason { get; set; } = string.Empty;

    [Required, Display(Name = "Start date/time")]
    public DateTime StartDateTime { get; set; } = DateTime.Now;

    [Required, Display(Name = "End date/time")]
    public DateTime EndDateTime { get; set; } = DateTime.Now.AddHours(8);

    public List<SelectListItem> SpaceTypeOptions { get; set; } = [];

    // Available spaces to select from (filtered by site)
    public List<LendableSpaceViewModel> AvailableSpaces { get; set; } = [];
}

public class LendableSpaceViewModel
{
    public int ParkingSpaceId { get; set; }
    public string SpaceNumber { get; set; } = string.Empty;
    public ParkingSpaceType CurrentType { get; set; }
    public bool IsCurrentlyLent { get; set; }
    public bool IsActive { get; set; }
}

public class ActiveLendingViewModel
{
    public int LendingId { get; set; }
    public string SpaceNumber { get; set; } = string.Empty;
    public ParkingSpaceType OriginalType { get; set; }
    public ParkingSpaceType LentToType { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public bool IsExpired => DateTime.Now > EndDateTime;
}

public class InactiveNoteViewModel
{
    public int ParkingSpaceId { get; set; }
    public string SpaceNumber { get; set; } = string.Empty;

    [Required, StringLength(500), Display(Name = "Reason for deactivation")]
    public string InactiveNote { get; set; } = string.Empty;

    [Display(Name = "Deactivation type")]
    public bool IsInactivePermanent { get; set; }

    public string SiteName { get; set; } = string.Empty;
    public int SiteId { get; set; }
}