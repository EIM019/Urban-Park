using System.ComponentModel.DataAnnotations;
using CarparkManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarparkManagementSystem.ViewModels.Availability;

public class AvailabilitySearchViewModel : IValidatableObject
{
    [Display(Name = "Site")]
    public int? SiteId { get; set; }

    [Display(Name = "Start")]
    [DataType(DataType.DateTime)]
    public DateTime? StartDateTime { get; set; }

    [Display(Name = "End")]
    [DataType(DataType.DateTime)]
    public DateTime? EndDateTime { get; set; }

    [Display(Name = "Bay type")]
    public ParkingSpaceType? SpaceType { get; set; }

    public bool HasSearched { get; set; }

    public bool CanCreateBookings { get; set; }

    public string StatusMessage { get; set; } = string.Empty;

    public List<SelectListItem> SiteOptions { get; set; } = [];

    public List<SelectListItem> SpaceTypeOptions { get; set; } = [];

    public List<AvailableSpaceViewModel> Results { get; set; } = [];

    public List<InactiveSpaceNoteViewModel> InactiveSpacesWithNotes { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!HasSearched)
        {
            yield break;
        }

        if (SiteId is null)
        {
            yield return new ValidationResult("Choose a site before searching.", [nameof(SiteId)]);
        }

        if (StartDateTime is null)
        {
            yield return new ValidationResult("Choose a start date and time.", [nameof(StartDateTime)]);
        }

        if (EndDateTime is null)
        {
            yield return new ValidationResult("Choose an end date and time.", [nameof(EndDateTime)]);
        }

        if (StartDateTime is not null && EndDateTime is not null && EndDateTime <= StartDateTime)
        {
            yield return new ValidationResult("End time must be after the start time.", [nameof(EndDateTime)]);
        }
    }
}

public class AvailableSpaceViewModel
{
    public int ParkingSpaceId { get; init; }

    public int SiteId { get; init; }

    public string SiteName { get; init; } = string.Empty;

    public string SpaceNumber { get; init; } = string.Empty;

    public ParkingSpaceType SpaceType { get; init; }

    public string AvailabilityNote { get; init; } = string.Empty;
}

// Add to AvailabilitySearchViewModel.cs or a shared file
public class InactiveSpaceNoteViewModel
{
    public string SpaceNumber       { get; set; } = string.Empty;
    public string SiteName          { get; set; } = string.Empty;
    public string Note              { get; set; } = string.Empty;
    public bool   IsPermanent       { get; set; }
    public string StatusLabel       => IsPermanent ? "Permanent" : "Temporary";
    public DateTime? InactiveSince  { get; set; }
}
