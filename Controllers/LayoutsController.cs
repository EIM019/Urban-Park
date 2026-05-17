using CarparkManagementSystem.Data; 
using CarparkManagementSystem.Constants;
using CarparkManagementSystem.Services;
using CarparkManagementSystem.ViewModels.Layouts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarparkManagementSystem.Controllers;

[Authorize]
public class LayoutsController(ILayoutService layoutService, ApplicationDbContext context) : Controller
{
    private readonly ILayoutService _layoutService = layoutService;
     private readonly ApplicationDbContext _context = context;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = await _layoutService.GetLayoutsIndexAsync(User, cancellationToken);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, DateTime? startDateTime, DateTime? endDateTime, CancellationToken cancellationToken)
    {
        var model = await _layoutService.GetSiteLayoutAsync(id, startDateTime, endDateTime, User, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [Authorize(Roles = $"{ApplicationRoles.FacilitiesManager},{ApplicationRoles.ITTechnician}")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var model = await _layoutService.GetLayoutEditorAsync(id, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [Authorize(Roles = $"{ApplicationRoles.FacilitiesManager},{ApplicationRoles.ITTechnician}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LayoutEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
             var errors = ModelState
                .Where(x => x.Value!.Errors.Any())
                .Select(x => $"{x.Key}: {string.Join(", ", x.Value!.Errors.Select(e => e.ErrorMessage))}");
            foreach (var e in errors) Console.WriteLine($"VALIDATION: {e}");
            return View(model);
        }

        var result = await _layoutService.UpdateLayoutAsync(model, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Details), new { id = model.SiteId });
    }

    // GET: show active lendings and lending form for a site
    [HttpGet]
    [Authorize(Roles = $"{ApplicationRoles.ITTechnician},{ApplicationRoles.FacilitiesManager}")]
    public async Task<IActionResult> LendSpaces(int siteId, CancellationToken cancellationToken)
    {
        // Auto-revert any expired lendings on every visit
        await _layoutService.ProcessExpiredLendingsAsync(cancellationToken);

        var model = await _layoutService.BuildLendingFormAsync(siteId, User, cancellationToken);
        if (model is null) return Forbid();

        // Load active lendings for this site to display in the table
        ViewBag.ActiveLendings = await _context.SpaceLendings
            .AsNoTracking()
            .Include(l => l.ParkingSpace)
            .Where(l => l.ParkingSpace!.SiteId == siteId && !l.IsReverted && l.EndDateTime > DateTime.Now)
            .Select(l => new ActiveLendingViewModel
            {
                LendingId        = l.Id,
                SpaceNumber      = l.ParkingSpace!.SpaceNumber,
                OriginalType     = l.OriginalType,
                LentToType       = l.LentToType,
                Reason           = l.Reason,
                StartDateTime    = l.StartDateTime,
                EndDateTime      = l.EndDateTime,
                CreatedByUserName= l.CreatedByUserName
            }).ToListAsync(cancellationToken);

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = $"{ApplicationRoles.ITTechnician},{ApplicationRoles.FacilitiesManager}")]
    public async Task<IActionResult> LendSpaces(SpaceLendingFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _layoutService.CreateSpaceLendingAsync(model, User, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Details), new { id = model.SiteId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = $"{ApplicationRoles.ITTechnician},{ApplicationRoles.FacilitiesManager}")]
    public async Task<IActionResult> RevertLending(int id, int siteId, CancellationToken cancellationToken)
    {
        var result = await _layoutService.RevertLendingAsync(id, User, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Details), new { id = siteId });
    }

    // GET: deactivate space with note
    [HttpGet]
    [Authorize(Roles = $"{ApplicationRoles.ITTechnician},{ApplicationRoles.FacilitiesManager}")]
    public async Task<IActionResult> DeactivateSpace(int spaceId, CancellationToken cancellationToken)
    {
        var space = await _context.ParkingSpaces
            .AsNoTracking()
            .Include(s => s.Site)
            .FirstOrDefaultAsync(s => s.Id == spaceId, cancellationToken);

        if (space is null) return NotFound();

        return View(new InactiveNoteViewModel
        {
            ParkingSpaceId = space.Id,
            SpaceNumber    = space.SpaceNumber,
            SiteId         = space.SiteId,
            SiteName       = space.Site?.Name ?? string.Empty
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = $"{ApplicationRoles.ITTechnician},{ApplicationRoles.FacilitiesManager}")]
    public async Task<IActionResult> DeactivateSpace(InactiveNoteViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _layoutService.SetInactiveNoteAsync(model, User, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Details), new { id = model.SiteId });
    }
}
