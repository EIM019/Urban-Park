using System.ComponentModel.DataAnnotations;

namespace CarparkManagementSystem.Models;

public class SpaceLending
{
    public int Id { get; set; }
    public int ParkingSpaceId { get; set; }

    // Original type before lending
    public ParkingSpaceType OriginalType { get; set; }

    // Temporarily lent to this type
    public ParkingSpaceType LentToType { get; set; }

    [Required, StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    public bool IsReverted { get; set; } = false;
    public DateTime? RevertedAt { get; set; }
    public string? RevertedByUserId { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public string CreatedByUserId { get; set; } = string.Empty;
    public string CreatedByUserName { get; set; } = string.Empty;

    // Navigation
    public ParkingSpace? ParkingSpace { get; set; }
}