using System.Diagnostics;
using CarparkManagementSystem.Constants;
using CarparkManagementSystem.Models;
using CarparkManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarparkManagementSystem.Controllers;

public class HomeController(ILogger<HomeController> logger, IBookingService bookingService) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IBookingService _bookingService = bookingService;

    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToRoleDashboard();
        }

        return View();
    }

    [Authorize(Roles = ApplicationRoles.ReceptionistAdmin)]
    public async Task<IActionResult> ReceptionistDashboard(CancellationToken cancellationToken)
    {
        var model = await _bookingService.GetReceptionistDashboardAsync(User, cancellationToken);
        return View(model);
    }

    [Authorize(Roles = ApplicationRoles.FacilitiesManager)]
    public async Task<IActionResult> ManagerDashboard(CancellationToken cancellationToken)
    {
        var model = await _bookingService.GetManagerDashboardAsync(User, cancellationToken);
        return View(model);
    }

    [Authorize(Roles = ApplicationRoles.ITTechnician)]
    public async Task<IActionResult> ITDashboard(CancellationToken cancellationToken)
    {
        var model = await _bookingService.GetITDashboardAsync(User, cancellationToken);
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> UserDashboard(CancellationToken cancellationToken)
    {
        var model = await _bookingService.GetUserDashboardAsync(User, cancellationToken);
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogError("Unhandled request reached the error page. RequestId: {RequestId}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private IActionResult RedirectToRoleDashboard()
    {
        if (User.IsInRole(ApplicationRoles.FacilitiesManager))
        {
            return RedirectToAction(nameof(ManagerDashboard));
        }

        if (User.IsInRole(ApplicationRoles.ReceptionistAdmin))
        {
            return RedirectToAction(nameof(ReceptionistDashboard));
        }

        if (User.IsInRole(ApplicationRoles.ITTechnician))
        {
            return RedirectToAction(nameof(ITDashboard));
        }

        return RedirectToAction(nameof(UserDashboard));
    }
}
