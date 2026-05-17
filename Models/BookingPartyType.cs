using System.ComponentModel.DataAnnotations;

namespace CarparkManagementSystem.Models;

public enum BookingPartyType
{
    [Display(Name = "Employee")]
    Employee = 0,

    [Display(Name = "Visitor")]
    Visitor = 1
}
