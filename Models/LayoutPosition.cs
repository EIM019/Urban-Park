namespace CarparkManagementSystem.Models;

public class LayoutPosition
{
    public int Id { get; set; }

    public int ParkingSpaceId { get; set; }

    public int Row { get; set; }

    public int Column { get; set; }
    public string? CardBackgroundColor { get; set; }
    public string? CardTextColor { get; set; }
    public string? CardFontWeight { get; set; }

    public ParkingSpace? ParkingSpace { get; set; }
}
