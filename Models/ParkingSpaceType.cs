using System.ComponentModel.DataAnnotations;

namespace CarparkManagementSystem.Models;

public enum ParkingSpaceType
{
    [Display(Name = "Standard")]
    Standard = 0,

    [Display(Name = "EV Charging")]
    ElectricCharging = 1,

    [Display(Name = "Disabled")]
    Disabled = 2,

    [Display(Name = "Visitor")]
    Visitor = 3
}
