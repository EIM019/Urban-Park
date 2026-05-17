using System.Security.Claims;
using CarparkManagementSystem.Constants;
using CarparkManagementSystem.Data;
using CarparkManagementSystem.Models;
using CarparkManagementSystem.ViewModels.Layouts;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.EntityFrameworkCore;

namespace CarparkManagementSystem.Services;

public class LayoutService(ApplicationDbContext context) : ILayoutService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<LayoutsIndexViewModel> GetLayoutsIndexAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var sites = await _context.Sites
            .AsNoTracking()
            .Include(site => site.ParkingSpaces)
            .OrderBy(site => site.Name)
            .ToListAsync(cancellationToken);

        return new LayoutsIndexViewModel
        {
            CanEditLayouts = CanEditLayouts(user),
            Sites = sites.Select(site => new LayoutSiteCardViewModel
            {
                SiteId = site.Id,
                Name = site.Name,
                TotalSpaces = site.TotalSpaces,
                StandardSpaces = site.ParkingSpaces.Count(space => space.Type == ParkingSpaceType.Standard),
                ElectricChargingSpaces = site.ParkingSpaces.Count(space => space.Type == ParkingSpaceType.ElectricCharging),
                DisabledSpaces = site.ParkingSpaces.Count(space => space.Type == ParkingSpaceType.Disabled),
                VisitorSpaces = site.ParkingSpaces.Count(space => space.Type == ParkingSpaceType.Visitor)
            }).ToList()
        };
    }

    public async Task<SiteLayoutViewModel?> GetSiteLayoutAsync(
        int siteId,
        DateTime? startDateTime,
        DateTime? endDateTime,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var windowStart = startDateTime ?? DateTime.Now;
        var windowEnd = endDateTime ?? windowStart.AddHours(8);

        var site = await _context.Sites
            .AsNoTracking()
            .Include(existing => existing.ParkingSpaces)
                .ThenInclude(space => space.LayoutPosition)
            .Include(existing => existing.ParkingSpaces)
                .ThenInclude(space => space.Bookings)
            .FirstOrDefaultAsync(existing => existing.Id == siteId, cancellationToken);

        if (site is null)
        {
            return null;
        }

        var orderedSpaces = site.ParkingSpaces
            .OrderBy(space => space.LayoutPosition?.Row ?? int.MaxValue)
            .ThenBy(space => space.LayoutPosition?.Column ?? int.MaxValue)
            .ThenBy(space => space.SpaceNumber)
            .ToList();

        var mappedSpaces = orderedSpaces.Select(space =>
        {
            var activeBooking = space.Bookings
                .Where(booking => booking.Status == BookingStatus.Active
                            && booking.StartDateTime < windowEnd
                            && booking.EndDateTime > windowStart)
                .OrderBy(booking => booking.StartDateTime)
                .FirstOrDefault();

            return new LayoutSpaceViewModel
            {
                ParkingSpaceId      = space.Id,
                SpaceNumber         = space.SpaceNumber,
                SpaceType           = space.Type,
                Row                 = space.LayoutPosition?.Row ?? 1,
                Column              = space.LayoutPosition?.Column ?? 1,
                IsActive            = space.IsActive,
                IsAvailable         = space.IsActive && activeBooking is null,
                IsPriorityBooking   = activeBooking?.IsPriority == true,
                BookingLabel        = activeBooking is null
                                        ? "Available"
                                        : $"{activeBooking.RequesterName} ({activeBooking.StartDateTime:g} - {activeBooking.EndDateTime:g})",
                InactiveNote        = space.InactiveNote,           // ← new
                IsInactivePermanent = space.IsInactivePermanent,    // ← new
                InactiveSince       = space.InactiveSince           // ← new
            };
        }).ToList();

        return new SiteLayoutViewModel
        {
            SiteId = site.Id,
            SiteName = site.Name,
            StartDateTime = windowStart,
            EndDateTime = windowEnd,
            GridColumns = Math.Max(mappedSpaces.Max(space => space.Column), 1),
            CanEdit = CanEditLayouts(user),
            TotalSpaces = mappedSpaces.Count,
            AvailableSpaces = mappedSpaces.Count(space => space.IsAvailable),
            BookedSpaces = mappedSpaces.Count(space => !space.IsAvailable && space.IsActive),
            PriorityBookedSpaces = mappedSpaces.Count(space => space.IsPriorityBooking),
            Spaces = mappedSpaces
        };
    }

    public async Task<SpaceLendingFormViewModel?> BuildLendingFormAsync(
    int siteId, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        if (!CanEditLayouts(user)) return null;

        var site = await _context.Sites
            .AsNoTracking()
            .Include(s => s.ParkingSpaces)
            .FirstOrDefaultAsync(s => s.Id == siteId, cancellationToken);

        if (site is null) return null;

        var activeLendings = await _context.SpaceLendings
            .AsNoTracking()
            .Where(l => !l.IsReverted && l.EndDateTime > DateTime.Now)
            .Select(l => l.ParkingSpaceId)
            .ToListAsync(cancellationToken);

        return new SpaceLendingFormViewModel
        {
            SiteId   = site.Id,
            SiteName = site.Name,
            StartDateTime = DateTime.Now,
            EndDateTime   = DateTime.Now.AddHours(8),
            SpaceTypeOptions =
            [
                new SelectListItem(ParkingSpaceType.Standard.GetDisplayName(),        ParkingSpaceType.Standard.ToString()),
                new SelectListItem(ParkingSpaceType.ElectricCharging.GetDisplayName(),ParkingSpaceType.ElectricCharging.ToString()),
                new SelectListItem(ParkingSpaceType.Disabled.GetDisplayName(),        ParkingSpaceType.Disabled.ToString()),
                new SelectListItem(ParkingSpaceType.Visitor.GetDisplayName(),         ParkingSpaceType.Visitor.ToString())
            ],
            AvailableSpaces = site.ParkingSpaces.Select(s => new LendableSpaceViewModel
            {
                ParkingSpaceId  = s.Id,
                SpaceNumber     = s.SpaceNumber,
                CurrentType     = s.Type,
                IsActive        = s.IsActive,
                IsCurrentlyLent = activeLendings.Contains(s.Id)
            }).OrderBy(s => s.SpaceNumber).ToList()
        };
    }

    public async Task<ServiceResult> CreateSpaceLendingAsync(
        SpaceLendingFormViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        if (!CanEditLayouts(user))
            return ServiceResult.Failure("Only IT Technicians and Facilities Managers can lend spaces.");

        if (!model.SelectedSpaceIds.Any())
            return ServiceResult.Failure("Please select at least one space to lend.");

        if (model.EndDateTime <= model.StartDateTime)
            return ServiceResult.Failure("End date/time must be after start date/time.");

        var spaces = await _context.ParkingSpaces
            .Where(s => model.SelectedSpaceIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var userName = user.FindFirstValue(ClaimTypes.Name) ?? "IT Technician";
        var userId   = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        foreach (var space in spaces)
        {
            // Create lending record
            var lending = new SpaceLending
            {
                ParkingSpaceId   = space.Id,
                OriginalType     = space.Type,
                LentToType       = model.LendToType,
                Reason           = model.Reason.Trim(),
                StartDateTime    = model.StartDateTime,
                EndDateTime      = model.EndDateTime,
                CreatedByUserId  = userId,
                CreatedByUserName= userName,
                CreatedUtc       = DateTime.UtcNow
            };
            _context.SpaceLendings.Add(lending);

            // Change the space type immediately
            space.Type = model.LendToType;
        }

        // Notify managers and receptionists
        var recipients = await _context.Users
            .Where(u => _context.UserRoles
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .Any(ur => ur.UserId == u.Id &&
                        (ur.Name == ApplicationRoles.FacilitiesManager ||
                        ur.Name == ApplicationRoles.ReceptionistAdmin)))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        foreach (var recipientId in recipients)
        {
            _context.Notifications.Add(new Notification
            {
                UserId  = recipientId,
                Title   = "Parking spaces temporarily reassigned",
                Message = $"{userName} has lent {spaces.Count} space(s) at {model.SiteName} " +
                        $"to {model.LendToType.GetDisplayName()} use. " +
                        $"Reason: {model.Reason}. " +
                        $"Period: {model.StartDateTime:dd MMM HH:mm} – {model.EndDateTime:dd MMM HH:mm}. " +
                        $"Spaces will revert automatically when the period ends.",
                CreatedUtc = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success($"{spaces.Count} space(s) successfully lent to {model.LendToType.GetDisplayName()} use.");
    }

    public async Task<ServiceResult> RevertLendingAsync(
        int lendingId, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        if (!CanEditLayouts(user))
            return ServiceResult.Failure("You do not have permission to revert space lendings.");

        var lending = await _context.SpaceLendings
            .Include(l => l.ParkingSpace)
            .FirstOrDefaultAsync(l => l.Id == lendingId, cancellationToken);

        if (lending is null) return ServiceResult.Failure("Lending record not found.");
        if (lending.IsReverted) return ServiceResult.Failure("This lending has already been reverted.");

        lending.IsReverted       = true;
        lending.RevertedAt       = DateTime.UtcNow;
        lending.RevertedByUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (lending.ParkingSpace is not null)
            lending.ParkingSpace.Type = lending.OriginalType;

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success("Space successfully reverted to its original type.");
    }

    public async Task<ServiceResult> SetInactiveNoteAsync(
        InactiveNoteViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        if (!CanEditLayouts(user))
            return ServiceResult.Failure("You do not have permission to update spaces.");

        var space = await _context.ParkingSpaces
            .FirstOrDefaultAsync(s => s.Id == model.ParkingSpaceId, cancellationToken);

        if (space is null) return ServiceResult.Failure("Parking space not found.");

        space.IsActive            = false;
        space.InactiveNote        = model.InactiveNote.Trim();
        space.IsInactivePermanent = model.IsInactivePermanent;
        space.InactiveSince       = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success($"Space {space.SpaceNumber} marked as inactive.");
    }

    // Called by a background job or on page load to auto-revert expired lendings
    public async Task ProcessExpiredLendingsAsync(CancellationToken cancellationToken = default)
    {
        var expired = await _context.SpaceLendings
            .Include(l => l.ParkingSpace)
            .Where(l => !l.IsReverted && l.EndDateTime <= DateTime.Now)
            .ToListAsync(cancellationToken);

        foreach (var lending in expired)
        {
            lending.IsReverted = true;
            lending.RevertedAt = DateTime.UtcNow;
            if (lending.ParkingSpace is not null)
                lending.ParkingSpace.Type = lending.OriginalType;
        }

        if (expired.Any())
            await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<LayoutEditViewModel?> GetLayoutEditorAsync(int siteId, CancellationToken cancellationToken = default)
    {
        var site = await _context.Sites
            .AsNoTracking()
            .Include(existing => existing.ParkingSpaces)
                .ThenInclude(space => space.LayoutPosition)
            .FirstOrDefaultAsync(existing => existing.Id == siteId, cancellationToken);

        if (site is null)
        {
            return null;
        }

        return new LayoutEditViewModel
        {
            SiteId = site.Id,
            SiteName = site.Name,
            Spaces = site.ParkingSpaces
                .OrderBy(space => space.SpaceNumber)
                .Select(space => new LayoutEditItemViewModel
                {
                    ParkingSpaceId = space.Id,
                    SpaceNumber = space.SpaceNumber,
                    SpaceType = space.Type,
                    Row = space.LayoutPosition?.Row ?? 1,
                    Column = space.LayoutPosition?.Column ?? 1,
                    IsActive = space.IsActive
                })
                .ToList()
        };
    }

    public async Task<ServiceResult> UpdateLayoutAsync(LayoutEditViewModel model, CancellationToken cancellationToken = default)
    {
        var parkingSpaceIds = model.Spaces.Select(space => space.ParkingSpaceId).ToList();
        var spaces = await _context.ParkingSpaces
            .Include(space => space.LayoutPosition)
            .Where(space => parkingSpaceIds.Contains(space.Id))
            .ToListAsync(cancellationToken);

        foreach (var editableSpace in model.Spaces)
        {
            var entity = spaces.FirstOrDefault(space => space.Id == editableSpace.ParkingSpaceId);
            if (entity is null)
            {
                continue;
            }

            entity.IsActive = editableSpace.IsActive;

            if (entity.LayoutPosition is null)
            {
                entity.LayoutPosition = new LayoutPosition
                {
                    Row = editableSpace.Row,
                    Column = editableSpace.Column
                };
            }
            else
            {
                entity.LayoutPosition.Row = editableSpace.Row;
                entity.LayoutPosition.Column = editableSpace.Column;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success("Layout updated successfully.");
    }

    private static bool CanEditLayouts(ClaimsPrincipal user)
    {
        return user.IsInRole(ApplicationRoles.FacilitiesManager) || user.IsInRole(ApplicationRoles.ITTechnician);
    }
}
