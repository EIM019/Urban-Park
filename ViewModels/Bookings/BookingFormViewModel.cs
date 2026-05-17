using System.ComponentModel.DataAnnotations;
using CarparkManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarparkManagementSystem.ViewModels.Bookings;

public class BookingFormViewModel : IValidatableObject
{
    public int? Id { get; set; }

    public bool IsEditing { get; set; }

    public int SiteId { get; set; }

    public string SiteName { get; set; } = string.Empty;

    public int ParkingSpaceId { get; set; }

    public string SpaceNumber { get; set; } = string.Empty;

    public ParkingSpaceType SpaceType { get; set; }

    [Required]
    [Display(Name = "Start")]
    [DataType(DataType.DateTime)]
    public DateTime StartDateTime { get; set; }

    [Required]
    [Display(Name = "End")]
    [DataType(DataType.DateTime)]
    public DateTime EndDateTime { get; set; }

    [Required]
    [Display(Name = "Booking for")]
    public BookingPartyType PartyType { get; set; }

    [Required]
    [Display(Name = "Name")]
    [MaxLength(120)]
    public string RequesterName { get; set; } = string.Empty;

    [Display(Name = "Employee number")]
    [MaxLength(40)]
    public string? EmployeeNumber { get; set; }

    [Display(Name = "Company")]
    [MaxLength(120)]
    public string? Company { get; set; }

    [Required]
    [Display(Name = "Contact number")]
    [MaxLength(40)]
    public string ContactNumber { get; set; } = string.Empty;

    [Display(Name = "Priority booking")]
    public bool IsPriority { get; set; }

    public bool CanSetPriority { get; set; }

    public List<SelectListItem> PartyTypeOptions { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDateTime <= StartDateTime)
        {
            yield return new ValidationResult("End time must be after the start time.", [nameof(EndDateTime)]);
        }

        if (PartyType == BookingPartyType.Employee && string.IsNullOrWhiteSpace(EmployeeNumber))
        {
            yield return new ValidationResult("Employee number is required for staff bookings.", [nameof(EmployeeNumber)]);
        }

        if (PartyType == BookingPartyType.Visitor && string.IsNullOrWhiteSpace(Company))
        {
            yield return new ValidationResult("Company is required for visitor bookings.", [nameof(Company)]);
        }
    }
}
