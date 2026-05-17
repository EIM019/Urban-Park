using System.ComponentModel.DataAnnotations;

namespace CarparkManagementSystem.Models;

public class ParkingSpace
{
    public int Id { get; set; }

    public int SiteId { get; set; }

    [Required]
    [MaxLength(20)]
    public string SpaceNumber { get; set; } = string.Empty;

    public ParkingSpaceType Type { get; set; }

    public bool IsActive { get; set; } = true;

    public Site? Site { get; set; }
    public string? InactiveNote { get; set; }
    public bool IsInactivePermanent { get; set; } = false;
    public DateTime? InactiveSince { get; set; }
    public string? CardBackgroundColor { get; set; }
    public string? CardTextColor { get; set; }
    public string? CardFontWeight { get; set; }

    public LayoutPosition? LayoutPosition { get; set; }

    public ICollection<Booking> Bookings { get; set; } = [];
}
