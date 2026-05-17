using CarparkManagementSystem.Models;
using CarparkManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarparkManagementSystem.Controllers;

[Authorize]
public class AvailabilityController(IBookingService bookingService) : Controller
{
    private readonly IBookingService _bookingService = bookingService;

    [HttpGet]
    public async Task<IActionResult> Search(
        int? siteId,
        DateTime? startDateTime,
        DateTime? endDateTime,
        ParkingSpaceType? spaceType,
        CancellationToken cancellationToken)
    {
        var model = await _bookingService.BuildAvailabilitySearchAsync(
            siteId,
            startDateTime,
            endDateTime,
            spaceType,
            User,
            cancellationToken);

        return View(model);
    }
}
