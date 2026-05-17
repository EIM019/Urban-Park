using System.ComponentModel.DataAnnotations;

namespace CarparkManagementSystem.Models;

public class Site
{
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int TotalSpaces { get; set; }

    public ICollection<ParkingSpace> ParkingSpaces { get; set; } = [];
}
