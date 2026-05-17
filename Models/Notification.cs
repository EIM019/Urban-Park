// Models/Notification.cs
namespace CarparkManagementSystem.Models;

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;      // recipient
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public int? RelatedBookingId { get; set; }

    // Navigation
    public Booking? RelatedBooking { get; set; }
}