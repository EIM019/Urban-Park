using CarparkManagementSystem.Constants;
using CarparkManagementSystem.Services;
using CarparkManagementSystem.ViewModels.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarparkManagementSystem.Controllers;

[Authorize]
public class BookingsController(IBookingService bookingService) : Controller
{
    private readonly IBookingService _bookingService = bookingService;

    [HttpGet]
    public async Task<IActionResult> Index(int? siteId, string? status, CancellationToken cancellationToken)
    {
        var model = await _bookingService.GetBookingsAsync(siteId, status, User, cancellationToken);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(
        int siteId,
        int parkingSpaceId,
        DateTime startDateTime,
        DateTime endDateTime,
        CancellationToken cancellationToken)
    {
        var model = await _bookingService.GetCreateBookingAsync(siteId, parkingSpaceId, startDateTime, endDateTime, User, cancellationToken);
        if (model is null)
        {
            TempData["StatusMessage"] = "That space is no longer available for the chosen time.";
            return RedirectToAction("Search", "Availability", new { siteId, startDateTime, endDateTime });
        }

        return View("Editor", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingFormViewModel model, CancellationToken cancellationToken)
    {
        await _bookingService.PopulateBookingFormAsync(model, User, cancellationToken);
        if (!ModelState.IsValid)
        {
            return View("Editor", model);
        }

        var result = await _bookingService.CreateBookingAsync(model, User, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Editor", model);
        }

        TempData["StatusMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = $"{ApplicationRoles.FacilitiesManager},{ApplicationRoles.ReceptionistAdmin}")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var model = await _bookingService.GetEditBookingAsync(id, User, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        return View("Editor", model);
    }

    [Authorize(Roles = $"{ApplicationRoles.FacilitiesManager},{ApplicationRoles.ReceptionistAdmin}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookingFormViewModel model, CancellationToken cancellationToken)
    {
        model.IsEditing = true;
        await _bookingService.PopulateBookingFormAsync(model, User, cancellationToken);

        if (!ModelState.IsValid)
        {
            return View("Editor", model);
        }

        var result = await _bookingService.UpdateBookingAsync(model, User, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Editor", model);
        }

        TempData["StatusMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var result = await _bookingService.CancelBookingAsync(id, User, cancellationToken);
        TempData["StatusMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = ApplicationRoles.FacilitiesManager)]
    public async Task<IActionResult> Override(int id, CancellationToken cancellationToken)
    {
        var model = await _bookingService.BuildOverrideFormAsync(id, User, cancellationToken);
        if (model is null) return NotFound();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.FacilitiesManager)]
    public async Task<IActionResult> Override(OverrideBookingViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _bookingService.OverrideBookingAsync(model, User, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}
