using System.Security.Claims;
using CarparkManagementSystem.Constants;
using CarparkManagementSystem.Data;
using CarparkManagementSystem.Models;
using CarparkManagementSystem.ViewModels.Availability;
using CarparkManagementSystem.ViewModels.Bookings;
using CarparkManagementSystem.ViewModels.Home;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarparkManagementSystem.Services;

public class BookingService(ApplicationDbContext context) : IBookingService
{
    private readonly ApplicationDbContext _context = context;

    public Task<HomeDashboardViewModel> GetHomeDashboardAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new HomeDashboardViewModel());
    }

    public async Task<ReceptionistDashboardViewModel> GetReceptionistDashboardAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        var windowEnd = now.AddHours(8);
        var currentUserId = GetUserId(user);
        var unreadCount = await _context.Notifications
            .AsNoTracking()
            .CountAsync(n => n.UserId == currentUserId && !n.IsRead, cancellationToken);
        

        var sites = await _context.Sites
            .AsNoTracking()
            .Include(site => site.ParkingSpaces)
                .ThenInclude(space => space.Bookings)
            .OrderBy(site => site.Name)
            .ToListAsync(cancellationToken);

        var visitorSpaces = sites.SelectMany(site => site.ParkingSpaces)
            .Where(space => space.Type == ParkingSpaceType.Visitor && space.IsActive)
            .ToList();

        var siteCards = sites.Select(site =>
        {
            var activeSpaces = site.ParkingSpaces.Where(space => space.IsActive).ToList();
            var bookedCount = activeSpaces.Count(space => HasOverlap(space.Bookings, now, windowEnd));

            return new SiteSummaryCardViewModel
            {
                SiteId = site.Id,
                Name = site.Name,
                TotalSpaces = site.TotalSpaces,
                StandardSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Standard),
                ElectricChargingSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.ElectricCharging),
                DisabledSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Disabled),
                VisitorSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Visitor),
                AvailableInWindow = activeSpaces.Count - bookedCount
            };
        }).ToList();

        var upcomingVisitorBookings = await _context.Bookings
            .AsNoTracking()
            .Include(booking => booking.Site)
            .Include(booking => booking.ParkingSpace)
            .Where(booking => booking.PartyType == BookingPartyType.Visitor && booking.Status == BookingStatus.Active && booking.EndDateTime >= now)
            .OrderBy(booking => booking.StartDateTime)
            .Take(8)
            .ToListAsync(cancellationToken);

        return new ReceptionistDashboardViewModel
        {
            CurrentRole = "Receptionist/Admin",
            UnreadNotificationCount = unreadCount,
            VisitorBookingsToday = await _context.Bookings
                .AsNoTracking()
                .Where(booking => booking.PartyType == BookingPartyType.Visitor && booking.Status == BookingStatus.Active && booking.StartDateTime.Date == now.Date)
                .CountAsync(cancellationToken),
            OpenVisitorBookings = upcomingVisitorBookings.Count,
            AvailableVisitorSpaces = visitorSpaces.Count - visitorSpaces.Count(space => HasOverlap(space.Bookings, now, windowEnd)),
            Sites = siteCards,
            UpcomingVisitorBookings = upcomingVisitorBookings.Select(booking => new DashboardBookingViewModel
            {
                Id = booking.Id,
                SiteName = booking.Site?.Name ?? string.Empty,
                SpaceNumber = booking.ParkingSpace?.SpaceNumber ?? string.Empty,
                RequesterName = booking.RequesterName,
                PartyType = booking.PartyType,
                StartDateTime = booking.StartDateTime,
                EndDateTime = booking.EndDateTime,
                IsPriority = booking.IsPriority,
                
                Status = booking.Status
            }).ToList()
        };
    }

    public async Task<ManagerDashboardViewModel> GetManagerDashboardAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var currentUserId = GetUserId(user);

        var unreadCount = await _context.Notifications
            .AsNoTracking()
            .CountAsync(n => n.UserId == currentUserId && !n.IsRead, cancellationToken);

        var allBookings = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Site)
            .Include(b => b.ParkingSpace)
            .OrderByDescending(b => b.StartDateTime)
            .ToListAsync(cancellationToken);
            
        var now = DateTime.Now;
        var windowEnd = now.AddHours(8);

        var sites = await _context.Sites
            .AsNoTracking()
            .Include(site => site.ParkingSpaces)
                .ThenInclude(space => space.Bookings)
            .OrderBy(site => site.Name)
            .ToListAsync(cancellationToken);

        var siteCards = sites.Select(site =>
        {
            var activeSpaces = site.ParkingSpaces.Where(space => space.IsActive).ToList();
            var bookedCount = activeSpaces.Count(space => HasOverlap(space.Bookings, now, windowEnd));

            return new SiteSummaryCardViewModel
            {
                SiteId = site.Id,
                Name = site.Name,
                TotalSpaces = site.TotalSpaces,
                StandardSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Standard),
                ElectricChargingSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.ElectricCharging),
                DisabledSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Disabled),
                VisitorSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Visitor),
                AvailableInWindow = activeSpaces.Count - bookedCount
            };
        }).ToList();

        var priorityBookings = await _context.Bookings
            .AsNoTracking()
            .Include(booking => booking.Site)
            .Include(booking => booking.ParkingSpace)
            .Where(booking => booking.IsPriority && booking.Status == BookingStatus.Active && booking.EndDateTime >= now)
            .OrderBy(booking => booking.StartDateTime)
            .Take(8)
            .ToListAsync(cancellationToken);

        return new ManagerDashboardViewModel
        {
            CurrentRole = "Facilities Manager",
            PriorityBookingCount = priorityBookings.Count,
            SitesWithLowAvailability = siteCards.Count(card => card.AvailableInWindow < 5),
            Sites = siteCards,
            UnreadNotificationCount = unreadCount,
            UpcomingPriorityBookings = priorityBookings.Select(booking => new DashboardBookingViewModel
            {
                Id = booking.Id,
                SiteName = booking.Site?.Name ?? string.Empty,
                SpaceNumber = booking.ParkingSpace?.SpaceNumber ?? string.Empty,
                RequesterName = booking.RequesterName,
                PartyType = booking.PartyType,
                StartDateTime = booking.StartDateTime,
                EndDateTime = booking.EndDateTime,
                IsPriority = booking.IsPriority,
                Status = booking.Status
            }).ToList(),
            AllBookings = allBookings.Select(b => new ManagerBookingRowViewModel
            {
                Id = b.Id,
                SiteName = b.Site?.Name ?? string.Empty,
                SpaceNumber = b.ParkingSpace?.SpaceNumber ?? string.Empty,
                RequesterName = b.RequesterName,
                PartyTypeLabel = b.PartyType.GetDisplayName(),
                StartDisplay = b.StartDateTime.ToString("dd MMM yyyy HH:mm"),
                EndDisplay = b.EndDateTime.ToString("dd MMM yyyy HH:mm"),
                IsPriority = b.IsPriority,
                StatusLabel = b.Status.GetDisplayName(),
                StatusClass = b.Status switch
                {
                    BookingStatus.Active     => "status-badge--active",
                    BookingStatus.Cancelled  => "status-badge--cancelled",
                    BookingStatus.Overridden => "status-badge--overridden",
                    _                        => string.Empty
                },
                CreatedByName = b.CreatedByUserName,
                CreatedByRole = b.CreatedByUserRole,
                CanOverride = b.Status == BookingStatus.Active && !b.IsPriority,
                OverriddenRequesterName = b.OverriddenRequesterName,
                IsOverrideReplacement   = b.OverriddenBookingId.HasValue
            }).ToList()
        };
    }

    public async Task<ITDashboardViewModel> GetITDashboardAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        var windowEnd = now.AddHours(8);

        var sites = await _context.Sites
            .AsNoTracking()
            .Include(site => site.ParkingSpaces)
                .ThenInclude(space => space.Bookings)
            .Include(site => site.ParkingSpaces)
                .ThenInclude(space => space.LayoutPosition)
            .OrderBy(site => site.Name)
            .ToListAsync(cancellationToken);

        var siteCards = sites.Select(site =>
        {
            var activeSpaces = site.ParkingSpaces.Where(space => space.IsActive).ToList();
            var bookedCount = activeSpaces.Count(space => HasOverlap(space.Bookings, now, windowEnd));

            return new SiteSummaryCardViewModel
            {
                SiteId = site.Id,
                Name = site.Name,
                TotalSpaces = site.TotalSpaces,
                StandardSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Standard),
                ElectricChargingSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.ElectricCharging),
                DisabledSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Disabled),
                VisitorSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Visitor),
                AvailableInWindow = activeSpaces.Count - bookedCount
            };
        }).ToList();

        // Inactive and unpositioned counts
        var inactiveSpaces = await _context.ParkingSpaces
            .AsNoTracking()
            .CountAsync(space => !space.IsActive, cancellationToken);

        var unpositionedSpaces = await _context.ParkingSpaces
            .AsNoTracking()
            .CountAsync(space => space.LayoutPosition == null, cancellationToken);

        var sitesNeedingReview = sites.Count(site =>
            site.ParkingSpaces.Any(space => !space.IsActive || space.LayoutPosition is null));

        // Per-site health breakdown
        var siteHealthSummary = sites.Select(site => new SiteHealthViewModel
        {
            SiteId             = site.Id,
            SiteName           = site.Name,
            TotalSpaces        = site.ParkingSpaces.Count,
            InactiveSpaces     = site.ParkingSpaces.Count(s => !s.IsActive),
            UnpositionedSpaces = site.ParkingSpaces.Count(s => s.LayoutPosition is null)
        }).ToList();

        var totalSpaces  = sites.Sum(s => s.ParkingSpaces.Count);
        var healthySites = siteHealthSummary.Count(s => !s.HasIssues);

        // Dynamic system status message
        var systemStatus = (inactiveSpaces, unpositionedSpaces) switch
        {
            (0, 0) => "All systems healthy. No issues detected.",
            (> 0, 0) => $"{inactiveSpaces} inactive space(s) require attention.",
            (0, > 0) => $"{unpositionedSpaces} space(s) missing grid positions.",
            _ => $"{inactiveSpaces} inactive and {unpositionedSpaces} unpositioned space(s) need review."
        };

        var recentBookings = await _context.Bookings
            .AsNoTracking()
            .Include(booking => booking.Site)
            .Include(booking => booking.ParkingSpace)
            .Where(booking => booking.Status == BookingStatus.Active && booking.EndDateTime >= now)
            .OrderByDescending(booking => booking.CreatedUtc)
            .Take(8)
            .ToListAsync(cancellationToken);

        return new ITDashboardViewModel
        {
            CurrentRole              = "IT Technician",
            InactiveSpaces           = inactiveSpaces,
            UnpositionedSpaces       = unpositionedSpaces,
            SitesNeedingLayoutReview = sitesNeedingReview,
            TotalSites               = sites.Count,
            HealthySites             = healthySites,
            TotalSpaces              = totalSpaces,
            SystemStatusMessage      = systemStatus,
            SiteHealthSummary        = siteHealthSummary,
            Sites                    = siteCards,
            RecentBookings = recentBookings.Select(b => new ManagerBookingRowViewModel
            {
                Id             = b.Id,
                SiteName       = b.Site?.Name ?? string.Empty,
                SpaceNumber    = b.ParkingSpace?.SpaceNumber ?? string.Empty,
                RequesterName  = b.RequesterName,
                PartyTypeLabel = b.PartyType.GetDisplayName(),
                StartDisplay   = b.StartDateTime.ToString("dd MMM yyyy HH:mm"),
                EndDisplay     = b.EndDateTime.ToString("dd MMM yyyy HH:mm"),
                IsPriority     = b.IsPriority,
                StatusLabel    = b.Status.GetDisplayName(),
                StatusClass    = b.Status switch
                {
                    BookingStatus.Active     => "status-badge--active",
                    BookingStatus.Cancelled  => "status-badge--cancelled",
                    BookingStatus.Overridden => "status-badge--overridden",
                    _                        => string.Empty
                },
                CreatedByName           = b.CreatedByUserName,
                CreatedByRole           = b.CreatedByUserRole,
                CanOverride             = false, // IT cannot override
                OverriddenRequesterName = b.OverriddenRequesterName,
                IsOverrideReplacement   = b.OverriddenBookingId.HasValue
            }).ToList()
        };
    }

    public async Task<OverrideBookingViewModel?> BuildOverrideFormAsync(
        int bookingId, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        if (!user.IsInRole(ApplicationRoles.FacilitiesManager)) return null;

        var booking = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Site)
            .Include(b => b.ParkingSpace)
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

        if (booking is null || booking.Status != BookingStatus.Active) return null;

        return new OverrideBookingViewModel
        {
            OriginalBookingId = booking.Id,
            SiteName          = booking.Site?.Name ?? string.Empty,
            SpaceNumber       = booking.ParkingSpace?.SpaceNumber ?? string.Empty,
            OriginalRequester = booking.RequesterName,
            OriginalCreatedBy = $"{booking.CreatedByUserName} ({booking.CreatedByUserRole})",
            StartDateTime     = booking.StartDateTime,
            EndDateTime       = booking.EndDateTime,
            PartyType         = BookingPartyType.Employee,
            PartyTypeOptions  =
            [
                new SelectListItem(BookingPartyType.Employee.GetDisplayName(), BookingPartyType.Employee.ToString(), true),
                new SelectListItem(BookingPartyType.Visitor.GetDisplayName(),  BookingPartyType.Visitor.ToString())
            ]
        };
    }

    public async Task<ServiceResult> OverrideBookingAsync(
        OverrideBookingViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        if (!user.IsInRole(ApplicationRoles.FacilitiesManager))
            return ServiceResult.Failure("Only a Facilities Manager can override bookings.");

        var existing = await _context.Bookings
            .Include(b => b.ParkingSpace)
                .ThenInclude(s => s!.Bookings)
            .FirstOrDefaultAsync(b => b.Id == model.OriginalBookingId, cancellationToken);

        if (existing is null)
            return ServiceResult.Failure("Booking not found.");

        if (existing.Status != BookingStatus.Active)
            return ServiceResult.Failure("Only active bookings can be overridden.");

        // Check the new window doesn't conflict with OTHER bookings on the same space
        var hasConflict = existing.ParkingSpace!.Bookings.Any(b =>
            b.Id != existing.Id &&
            b.Status == BookingStatus.Active &&
            b.StartDateTime < model.EndDateTime &&
            b.EndDateTime > model.StartDateTime);

        if (hasConflict)
            return ServiceResult.Failure("The new time window conflicts with another active booking on this space.");

        // 1. Mark original as Overridden (keeps audit trail distinct from Cancelled)
        existing.Status             = BookingStatus.Overridden;
        existing.LastUpdatedUtc     = DateTime.UtcNow;
        existing.LastUpdatedByUserId = GetUserId(user);

        // 2. Create replacement priority booking
        var managerName = user.FindFirstValue(ClaimTypes.Name) ?? "Manager";
        var replacement = new Booking
        {
            SiteId              = existing.SiteId,
            ParkingSpaceId      = existing.ParkingSpaceId,
            RequesterName       = model.RequesterName.Trim(),
            PartyType           = model.PartyType,
            EmployeeNumber      = model.PartyType == BookingPartyType.Employee ? model.EmployeeNumber?.Trim() : null,
            Company             = null,
            ContactNumber       = model.ContactNumber.Trim(),
            StartDateTime       = model.StartDateTime,
            EndDateTime         = model.EndDateTime,
            IsPriority          = true,
            Status              = BookingStatus.Active,
            CreatedUtc          = DateTime.UtcNow,
            LastUpdatedUtc      = DateTime.UtcNow,
            CreatedByUserId     = GetUserId(user),
            LastUpdatedByUserId = GetUserId(user),
            CreatedByUserName   = managerName,
            CreatedByUserRole   = ApplicationRoles.FacilitiesManager,
            OverriddenBookingId = existing.Id,
            OverriddenRequesterName = existing.RequesterName
        };

        _context.Bookings.Add(replacement);

        // 3. Notify the original booking's creator (receptionist/user)
        if (!string.IsNullOrEmpty(existing.CreatedByUserId))
        {
            var notification = new Notification
            {
                UserId  = existing.CreatedByUserId,
                Title   = "Booking overridden — action required",
                Message = $"Your booking for {existing.RequesterName} on space " +
                        $"{existing.ParkingSpace?.SpaceNumber} at {existing.Site?.Name} " +
                        $"({existing.StartDateTime:dd MMM HH:mm} – {existing.EndDateTime:dd MMM HH:mm}) " +
                        $"was overridden by {managerName} and reassigned as a priority booking. " +
                        $"Please search for an alternative space and rebook for {existing.RequesterName}.",
                RelatedBookingId = existing.Id,
                CreatedUtc       = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success($"Booking #{existing.Id} overridden and priority booking created.");
    }

    public async Task<AllBookingsManagerViewModel> GetAllBookingsForManagerAsync(
        ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var bookings = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Site)
            .Include(b => b.ParkingSpace)
            .OrderByDescending(b => b.StartDateTime)
            .ToListAsync(cancellationToken);

        var currentUserId = GetUserId(user);
        var unread = await _context.Notifications
            .AsNoTracking()
            .CountAsync(n => n.UserId == currentUserId && !n.IsRead, cancellationToken);

        return new AllBookingsManagerViewModel
        {
            UnreadNotificationCount = unread,
            Bookings = bookings.Select(b => new ManagerBookingRowViewModel
            {
                Id            = b.Id,
                SiteName      = b.Site?.Name ?? string.Empty,
                SpaceNumber   = b.ParkingSpace?.SpaceNumber ?? string.Empty,
                RequesterName = b.RequesterName,
                PartyTypeLabel= b.PartyType.GetDisplayName(),
                StartDisplay  = b.StartDateTime.ToString("dd MMM yyyy HH:mm"),
                EndDisplay    = b.EndDateTime.ToString("dd MMM yyyy HH:mm"),
                IsPriority    = b.IsPriority,
                StatusLabel   = b.Status.GetDisplayName(),
                StatusClass   = b.Status switch
                {
                    BookingStatus.Active     => "status-badge--active",
                    BookingStatus.Cancelled  => "status-badge--cancelled",
                    BookingStatus.Overridden => "status-badge--overridden",
                    _                        => string.Empty
                },
                CreatedByName = b.CreatedByUserName,
                CreatedByRole = b.CreatedByUserRole,
                CanOverride   = b.Status == BookingStatus.Active && !b.IsPriority
            }).ToList()
        };
    }

    public async Task<AvailabilitySearchViewModel> BuildAvailabilitySearchAsync(
        int? siteId,
        DateTime? startDateTime,
        DateTime? endDateTime,
        ParkingSpaceType? spaceType,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var defaultStart = DateTime.Now;
        var defaultEnd = defaultStart.AddHours(8);
        var model = new AvailabilitySearchViewModel
        {
            SiteId = siteId,
            StartDateTime = startDateTime ?? defaultStart,
            EndDateTime = endDateTime ?? defaultEnd,
            SpaceType = spaceType,
            HasSearched = siteId.HasValue && startDateTime.HasValue && endDateTime.HasValue,
            CanCreateBookings = user.Identity?.IsAuthenticated == true
        };

        await PopulateAvailabilitySelectionsAsync(model, cancellationToken);

        if (!model.HasSearched)
        {
            model.StatusMessage = "Pick a site and time window to find matching spaces.";
            return model;
        }

        var spaces = await _context.ParkingSpaces
            .AsNoTracking()
            .Include(space => space.Site)
            .Include(space => space.Bookings)
            .Where(space => space.SiteId == model.SiteId && space.IsActive)
            .OrderBy(space => space.SpaceNumber)
            .ToListAsync(cancellationToken);

        if (model.SpaceType.HasValue)
        {
            spaces = spaces.Where(space => space.Type == model.SpaceType.Value).ToList();
        }

        model.Results = spaces
            .Where(space => !HasOverlap(space.Bookings, model.StartDateTime!.Value, model.EndDateTime!.Value))
            .Select(space => new AvailableSpaceViewModel
            {
                ParkingSpaceId = space.Id,
                SiteId = space.SiteId,
                SiteName = space.Site?.Name ?? string.Empty,
                SpaceNumber = space.SpaceNumber,
                SpaceType = space.Type,
                AvailabilityNote = $"{space.Type.GetDisplayName()} bay free for the full selected period."
            })
            .ToList();

       model.StatusMessage = model.Results.Count == 0
            ? "No spaces matched that request. Try another bay type or time window."
            : $"Found {model.Results.Count} available space(s).";

        // Populate inactive notes for the searched site
        if (model.SiteId.HasValue)
        {
            var inactiveWithNotes = await _context.ParkingSpaces
                .AsNoTracking()
                .Include(s => s.Site)
                .Where(s => s.SiteId == model.SiteId.Value
                        && !s.IsActive
                        && s.InactiveNote != null)
                .ToListAsync(cancellationToken);

            model.InactiveSpacesWithNotes = inactiveWithNotes.Select(s => new InactiveSpaceNoteViewModel
            {
                SpaceNumber   = s.SpaceNumber,
                SiteName      = s.Site?.Name ?? string.Empty,
                Note          = s.InactiveNote!,
                IsPermanent   = s.IsInactivePermanent,
                InactiveSince = s.InactiveSince
            }).ToList();
        }

        return model; 
    }

    public async Task<BookingsListViewModel> GetBookingsAsync(
        int? siteId,
        string? statusFilter,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(statusFilter) ? "Active" : statusFilter;
        var isPrivilegedUser = CanManageBookings(user) || user.IsInRole(ApplicationRoles.ITTechnician);
        var query = _context.Bookings
            .AsNoTracking()
            .Include(booking => booking.Site)
            .Include(booking => booking.ParkingSpace)
            .AsQueryable();

        if (!isPrivilegedUser)
        {
            var currentUserId = GetUserId(user);
            query = query.Where(booking => booking.CreatedByUserId == currentUserId);
        }

        if (siteId.HasValue)
        {
            query = query.Where(booking => booking.SiteId == siteId.Value);
        }

        query = normalizedStatus switch
        {
            "Cancelled" => query.Where(booking => booking.Status == BookingStatus.Cancelled),
            "All" => query,
            _ => query.Where(booking => booking.Status == BookingStatus.Active)
        };

        var bookings = await query
            .OrderBy(booking => booking.StartDateTime)
            .ToListAsync(cancellationToken);

        return new BookingsListViewModel
        {
            SiteId = siteId,
            StatusFilter = normalizedStatus,
            CanManageBookings = CanManageBookings(user),
            SiteOptions = await BuildSiteOptionsAsync(siteId, cancellationToken),
            StatusOptions =
            [
                new SelectListItem("Active", "Active", normalizedStatus == "Active"),
                new SelectListItem("Cancelled", "Cancelled", normalizedStatus == "Cancelled"),
                new SelectListItem("All", "All", normalizedStatus == "All")
            ],
            Bookings = bookings.Select(booking => new BookingListItemViewModel
            {
                Id = booking.Id,
                SiteName = booking.Site?.Name ?? string.Empty,
                SpaceNumber = booking.ParkingSpace?.SpaceNumber ?? string.Empty,
                SpaceType = booking.ParkingSpace?.Type ?? ParkingSpaceType.Standard,
                RequesterName = booking.RequesterName,
                PartyType = booking.PartyType,
                EmployeeNumber = booking.EmployeeNumber,
                Company = booking.Company,
                ContactNumber = booking.ContactNumber,
                StartDateTime = booking.StartDateTime,
                EndDateTime = booking.EndDateTime,
                IsPriority = booking.IsPriority,
                Status = booking.Status
            }).ToList()
        };
    }

    public async Task<BookingFormViewModel?> GetCreateBookingAsync(
        int siteId,
        int parkingSpaceId,
        DateTime startDateTime,
        DateTime endDateTime,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        if (!(user.Identity?.IsAuthenticated ?? false))
        {
            return null;
        }

        var parkingSpace = await _context.ParkingSpaces
            .AsNoTracking()
            .Include(space => space.Site)
            .Include(space => space.Bookings)
            .FirstOrDefaultAsync(space => space.Id == parkingSpaceId && space.SiteId == siteId, cancellationToken);

        if (parkingSpace is null || !parkingSpace.IsActive || HasOverlap(parkingSpace.Bookings, startDateTime, endDateTime))
        {
            return null;
        }

        var model = new BookingFormViewModel
        {
            IsEditing = false,
            SiteId = parkingSpace.SiteId,
            SiteName = parkingSpace.Site?.Name ?? string.Empty,
            ParkingSpaceId = parkingSpace.Id,
            SpaceNumber = parkingSpace.SpaceNumber,
            SpaceType = parkingSpace.Type,
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            PartyType = parkingSpace.Type == ParkingSpaceType.Visitor ? BookingPartyType.Visitor : BookingPartyType.Employee,
            CanSetPriority = user.IsInRole(ApplicationRoles.FacilitiesManager)
        };

        await PopulateBookingFormAsync(model, user, cancellationToken);
        return model;
    }

    public async Task<BookingFormViewModel?> GetEditBookingAsync(
        int id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        if (!CanManageBookings(user))
        {
            return null;
        }

        var booking = await _context.Bookings
            .AsNoTracking()
            .Include(existing => existing.Site)
            .Include(existing => existing.ParkingSpace)
            .FirstOrDefaultAsync(existing => existing.Id == id, cancellationToken);

        if (booking is null)
        {
            return null;
        }

        var model = new BookingFormViewModel
        {
            Id = booking.Id,
            IsEditing = true,
            SiteId = booking.SiteId,
            SiteName = booking.Site?.Name ?? string.Empty,
            ParkingSpaceId = booking.ParkingSpaceId,
            SpaceNumber = booking.ParkingSpace?.SpaceNumber ?? string.Empty,
            SpaceType = booking.ParkingSpace?.Type ?? ParkingSpaceType.Standard,
            StartDateTime = booking.StartDateTime,
            EndDateTime = booking.EndDateTime,
            PartyType = booking.PartyType,
            RequesterName = booking.RequesterName,
            EmployeeNumber = booking.EmployeeNumber,
            Company = booking.Company,
            ContactNumber = booking.ContactNumber,
            IsPriority = booking.IsPriority,
            CanSetPriority = user.IsInRole(ApplicationRoles.FacilitiesManager)
        };

        await PopulateBookingFormAsync(model, user, cancellationToken);
        return model;
    }

    public Task PopulateBookingFormAsync(BookingFormViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        model.CanSetPriority = user.IsInRole(ApplicationRoles.FacilitiesManager);
        model.PartyTypeOptions =
        [
            new SelectListItem(BookingPartyType.Employee.GetDisplayName(), BookingPartyType.Employee.ToString(), model.PartyType == BookingPartyType.Employee),
            new SelectListItem(BookingPartyType.Visitor.GetDisplayName(), BookingPartyType.Visitor.ToString(), model.PartyType == BookingPartyType.Visitor)
        ];

        return Task.CompletedTask;
    }

    public async Task<ServiceResult> CreateBookingAsync(BookingFormViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        if (!(user.Identity?.IsAuthenticated ?? false))
        {
            return ServiceResult.Failure("You must be signed in to create bookings.");
        }

        var parkingSpace = await _context.ParkingSpaces
            .Include(space => space.Bookings)
            .FirstOrDefaultAsync(space => space.Id == model.ParkingSpaceId && space.SiteId == model.SiteId, cancellationToken);

        if (parkingSpace is null || !parkingSpace.IsActive)
        {
            return ServiceResult.Failure("That parking space is no longer available.");
        }

        if (HasOverlap(parkingSpace.Bookings, model.StartDateTime, model.EndDateTime))
        {
            return ServiceResult.Failure("That space has already been booked for the selected time.");
        }

        if (model.IsPriority && !user.IsInRole(ApplicationRoles.FacilitiesManager))
        {
            return ServiceResult.Failure("Only the facilities manager can create priority bookings.");
        }

        var booking = new Booking
        {
            SiteId = model.SiteId,
            ParkingSpaceId = model.ParkingSpaceId,
            RequesterName = model.RequesterName.Trim(),
            PartyType = model.PartyType,
            EmployeeNumber = model.PartyType == BookingPartyType.Employee ? model.EmployeeNumber?.Trim() : null,
            Company = model.PartyType == BookingPartyType.Visitor ? model.Company?.Trim() : null,
            ContactNumber = model.ContactNumber.Trim(),
            StartDateTime = model.StartDateTime,
            EndDateTime = model.EndDateTime,
            IsPriority = model.IsPriority,
            Status = BookingStatus.Active,
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow,
            CreatedByUserId = GetUserId(user),
            LastUpdatedByUserId = GetUserId(user),
            CreatedByUserName = user.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            CreatedByUserRole = GetHighestRole(user)
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success("Booking created successfully.");
    }

    public async Task<ServiceResult> UpdateBookingAsync(BookingFormViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        if (!CanManageBookings(user))
        {
            return ServiceResult.Failure("You do not have permission to update bookings.");
        }

        var booking = await _context.Bookings
            .Include(existing => existing.ParkingSpace)
                .ThenInclude(space => space!.Bookings)
            .FirstOrDefaultAsync(existing => existing.Id == model.Id, cancellationToken);

        if (booking is null || booking.ParkingSpace is null)
        {
            return ServiceResult.Failure("The booking could not be found.");
        }

        var conflictingBooking = booking.ParkingSpace.Bookings.Any(existing =>
            existing.Id != booking.Id &&
            existing.Status == BookingStatus.Active &&
            existing.StartDateTime < model.EndDateTime &&
            existing.EndDateTime > model.StartDateTime);

        if (conflictingBooking)
        {
            return ServiceResult.Failure("The updated times overlap with another active booking.");
        }

        booking.RequesterName = model.RequesterName.Trim();
        booking.PartyType = model.PartyType;
        booking.EmployeeNumber = model.PartyType == BookingPartyType.Employee ? model.EmployeeNumber?.Trim() : null;
        booking.Company = model.PartyType == BookingPartyType.Visitor ? model.Company?.Trim() : null;
        booking.ContactNumber = model.ContactNumber.Trim();
        booking.StartDateTime = model.StartDateTime;
        booking.EndDateTime = model.EndDateTime;
        booking.IsPriority = user.IsInRole(ApplicationRoles.FacilitiesManager) ? model.IsPriority : booking.IsPriority;
        booking.LastUpdatedUtc = DateTime.UtcNow;
        booking.LastUpdatedByUserId = GetUserId(user);

        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success("Booking updated successfully.");
    }

    public async Task<ServiceResult> CancelBookingAsync(int id, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings.FirstOrDefaultAsync(existing => existing.Id == id, cancellationToken);
        if (booking is null)
            return ServiceResult.Failure("The booking could not be found.");

        var currentUserId = GetUserId(user);
        var isOwner = booking.CreatedByUserId == currentUserId;

        if (!CanManageBookings(user) && !isOwner)
            return ServiceResult.Failure("You do not have permission to cancel this booking.");

        if (booking.Status == BookingStatus.Cancelled)
            return ServiceResult.Failure("That booking is already cancelled.");

        booking.Status = BookingStatus.Cancelled;
        booking.LastUpdatedUtc = DateTime.UtcNow;
        booking.LastUpdatedByUserId = GetUserId(user);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success("Booking cancelled.");
    }

    private async Task PopulateAvailabilitySelectionsAsync(AvailabilitySearchViewModel model, CancellationToken cancellationToken)
    {
        model.SiteOptions = await BuildSiteOptionsAsync(model.SiteId, cancellationToken, includePrompt: true);
        model.SpaceTypeOptions =
        [
            new SelectListItem("Any bay type", string.Empty, !model.SpaceType.HasValue),
            new SelectListItem(ParkingSpaceType.Standard.GetDisplayName(), ParkingSpaceType.Standard.ToString(), model.SpaceType == ParkingSpaceType.Standard),
            new SelectListItem(ParkingSpaceType.ElectricCharging.GetDisplayName(), ParkingSpaceType.ElectricCharging.ToString(), model.SpaceType == ParkingSpaceType.ElectricCharging),
            new SelectListItem(ParkingSpaceType.Disabled.GetDisplayName(), ParkingSpaceType.Disabled.ToString(), model.SpaceType == ParkingSpaceType.Disabled),
            new SelectListItem(ParkingSpaceType.Visitor.GetDisplayName(), ParkingSpaceType.Visitor.ToString(), model.SpaceType == ParkingSpaceType.Visitor)
        ];
    }

    private async Task<List<SelectListItem>> BuildSiteOptionsAsync(int? selectedSiteId, CancellationToken cancellationToken, bool includePrompt = false)
    {
        var siteOptions = await _context.Sites
            .AsNoTracking()
            .OrderBy(site => site.Name)
            .Select(site => new SelectListItem(site.Name, site.Id.ToString(), site.Id == selectedSiteId))
            .ToListAsync(cancellationToken);

        if (includePrompt)
        {
            siteOptions.Insert(0, new SelectListItem("Choose a site", string.Empty, !selectedSiteId.HasValue));
        }

        return siteOptions;
    }

    private static bool HasOverlap(IEnumerable<Booking> bookings, DateTime startDateTime, DateTime endDateTime)
    {
        return bookings.Any(booking =>
            booking.Status == BookingStatus.Active &&
            booking.StartDateTime < endDateTime &&
            booking.EndDateTime > startDateTime);
    }

    public async Task<UserDashboardViewModel> GetUserDashboardAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        var windowEnd = now.AddHours(8);

        var sites = await _context.Sites
            .AsNoTracking()
            .Include(site => site.ParkingSpaces)
                .ThenInclude(space => space.Bookings)
            .OrderBy(site => site.Name)
            .ToListAsync(cancellationToken);

        var siteCards = sites.Select(site =>
        {
            var activeSpaces = site.ParkingSpaces.Where(space => space.IsActive).ToList();
            var bookedCount = activeSpaces.Count(space => HasOverlap(space.Bookings, now, windowEnd));

            return new SiteSummaryCardViewModel
            {
                SiteId = site.Id,
                Name = site.Name,
                TotalSpaces = site.TotalSpaces,
                StandardSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Standard),
                ElectricChargingSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.ElectricCharging),
                DisabledSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Disabled),
                VisitorSpaces = activeSpaces.Count(space => space.Type == ParkingSpaceType.Visitor),
                AvailableInWindow = activeSpaces.Count - bookedCount
            };
        }).ToList();

        var currentUserId = GetUserId(user);
        var myBookings = await _context.Bookings
            .AsNoTracking()
            .Include(booking => booking.Site)
            .Include(booking => booking.ParkingSpace)
            .Where(booking => booking.CreatedByUserId == currentUserId)
            .OrderByDescending(booking => booking.StartDateTime)
            .ToListAsync(cancellationToken);

        return new UserDashboardViewModel
        {
            SiteCount = sites.Count,
            TotalSpaceCount = siteCards.Sum(site => site.TotalSpaces),
            AvailableSpaceCount = siteCards.Sum(site => site.AvailableInWindow),
            UpcomingBookingCount = myBookings.Count(booking => booking.StartDateTime >= now && booking.Status == BookingStatus.Active),
            PastBookingCount = myBookings.Count(booking => booking.EndDateTime < now),
            MyBookings = myBookings.Select(booking => new DashboardBookingViewModel
            {
                Id = booking.Id,
                SiteName = booking.Site?.Name ?? string.Empty,
                SpaceNumber = booking.ParkingSpace?.SpaceNumber ?? string.Empty,
                RequesterName = booking.RequesterName,
                PartyType = booking.PartyType,
                StartDateTime = booking.StartDateTime,
                EndDateTime = booking.EndDateTime,
                IsPriority = booking.IsPriority,
                Status = booking.Status
            }).ToList()
        };
    }

    private static bool CanManageBookings(ClaimsPrincipal user)
    {
        return user.IsInRole(ApplicationRoles.FacilitiesManager) || user.IsInRole(ApplicationRoles.ReceptionistAdmin) || user.IsInRole(ApplicationRoles.ITTechnician);
    }

    private static string GetHighestRole(ClaimsPrincipal user)
    {
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return "Guest";
        }

        if (user.IsInRole(ApplicationRoles.FacilitiesManager))
        {
            return "Facilities Manager";
        }

        if (user.IsInRole(ApplicationRoles.ReceptionistAdmin))
        {
            return "Receptionist/Admin";
        }

        if (user.IsInRole(ApplicationRoles.ITTechnician))
        {
            return "IT Technician";
        }

        return "User";
    }

    private static string GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}
