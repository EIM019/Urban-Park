using System.ComponentModel.DataAnnotations;

namespace CarparkManagementSystem.Models;

public enum BookingStatus
{
    [Display(Name = "Active")]
    Active = 0,

    [Display(Name = "Cancelled")]
    Cancelled = 1,

    [Display(Name = "Overridden")]
    Overridden = 2
}
