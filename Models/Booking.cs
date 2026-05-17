using System.ComponentModel.DataAnnotations;

namespace CarparkManagementSystem.Models;

public class Booking
{
    public int Id { get; set; }

    public int SiteId { get; set; }

    public int ParkingSpaceId { get; set; }

    [Required]
    [MaxLength(120)]
    public string RequesterName { get; set; } = string.Empty;

    public BookingPartyType PartyType { get; set; }

    [MaxLength(40)]
    public string? EmployeeNumber { get; set; }

    [MaxLength(120)]
    public string? Company { get; set; }

    [Required]
    [MaxLength(40)]
    public string ContactNumber { get; set; } = string.Empty;

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public bool IsPriority { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Active;

    public DateTime CreatedUtc { get; set; }

    public DateTime? LastUpdatedUtc { get; set; }

    [MaxLength(450)]
    public string CreatedByUserId { get; set; } = string.Empty;

    [MaxLength(450)]
    public string? LastUpdatedByUserId { get; set; }

    public Site? Site { get; set; }

    public ParkingSpace? ParkingSpace { get; set; }

    public string CreatedByUserName { get; set; } = string.Empty;

    public string CreatedByUserRole { get; set; } = string.Empty;

    public int? OverriddenBookingId { get; set; }          // the replacement points back to the original
    public string? OverriddenRequesterName { get; set; }
}
