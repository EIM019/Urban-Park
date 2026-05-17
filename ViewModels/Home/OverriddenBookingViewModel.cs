using System.ComponentModel.DataAnnotations;
using CarparkManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarparkManagementSystem.ViewModels.Bookings;

public class OverrideBookingViewModel
{
    public int OriginalBookingId { get; set; }

    // Context (read-only display)
    public string SiteName          { get; set; } = string.Empty;
    public string SpaceNumber       { get; set; } = string.Empty;
    public string OriginalRequester { get; set; } = string.Empty;
    public string OriginalCreatedBy { get; set; } = string.Empty;

    // Manager picks new window + requester
    [Required, Display(Name = "Start date/time")]
    public DateTime StartDateTime { get; set; }

    [Required, Display(Name = "End date/time")]
    public DateTime EndDateTime { get; set; }

    [Required, StringLength(100), Display(Name = "Priority requester name")]
    public string RequesterName { get; set; } = string.Empty;

    [Required, Display(Name = "Party type")]
    public BookingPartyType PartyType { get; set; }

    [Display(Name = "Employee number")]
    public string? EmployeeNumber { get; set; }

    [Required, Phone, StringLength(30), Display(Name = "Contact number")]
    public string ContactNumber { get; set; } = string.Empty;

    public List<SelectListItem> PartyTypeOptions { get; set; } = [];
}