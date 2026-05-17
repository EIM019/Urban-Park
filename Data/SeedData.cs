using CarparkManagementSystem.Constants;
using CarparkManagementSystem.Models;
using CarparkManagementSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarparkManagementSystem.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var context = scopedProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scopedProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await context.Database.MigrateAsync();

        foreach (var role in ApplicationRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await EnsureUserAsync(userManager, ApplicationRoles.FacilitiesManager, "manager@roppacorp.local");
        await EnsureUserAsync(userManager, ApplicationRoles.ReceptionistAdmin, "reception@roppacorp.local");
        await EnsureUserAsync(userManager, ApplicationRoles.ITTechnician, "tech@roppacorp.local");

        if (!await context.Sites.AnyAsync())
        {
            var sites = new[]
            {
                new Site { Code = "A", Name = "Site A", TotalSpaces = 127 },
                new Site { Code = "B", Name = "Site B", TotalSpaces = 103 },
                new Site { Code = "C", Name = "Site C", TotalSpaces = 48 }
            };

            context.Sites.AddRange(sites);
            await context.SaveChangesAsync();

            CreateParkingSpaces(context, sites[0], electricCount: 21, disabledCount: 16, visitorCount: 12, columns: 12);
            CreateParkingSpaces(context, sites[1], electricCount: 14, disabledCount: 10, visitorCount: 8, columns: 11);
            CreateParkingSpaces(context, sites[2], electricCount: 12, disabledCount: 8, visitorCount: 8, columns: 8);

            await context.SaveChangesAsync();
        }

        if (!await context.Bookings.AnyAsync())
        {
            var manager = await userManager.FindByEmailAsync("manager@roppacorp.local");
            var receptionist = await userManager.FindByEmailAsync("reception@roppacorp.local");
            var (demoStart, _) = DemoWindowProvider.GetDefaultWindow();

            var siteASpace = await context.ParkingSpaces.Include(space => space.Site).FirstAsync(space => space.Site!.Code == "A" && space.Type == ParkingSpaceType.ElectricCharging);
            var siteBSpace = await context.ParkingSpaces.Include(space => space.Site).FirstAsync(space => space.Site!.Code == "B" && space.Type == ParkingSpaceType.Visitor);
            var siteCSpace = await context.ParkingSpaces.Include(space => space.Site).FirstAsync(space => space.Site!.Code == "C" && space.Type == ParkingSpaceType.Disabled);

            context.Bookings.AddRange(
                new Booking
                {
                    SiteId = siteASpace.SiteId,
                    ParkingSpaceId = siteASpace.Id,
                    RequesterName = "Operations Director",
                    PartyType = BookingPartyType.Employee,
                    EmployeeNumber = "FM1001",
                    ContactNumber = "+267 555 0101",
                    StartDateTime = demoStart,
                    EndDateTime = demoStart.AddHours(9),
                    IsPriority = true,
                    Status = BookingStatus.Active,
                    CreatedUtc = DateTime.UtcNow,
                    LastUpdatedUtc = DateTime.UtcNow,
                    CreatedByUserId = manager?.Id ?? string.Empty,
                    LastUpdatedByUserId = manager?.Id ?? string.Empty
                },
                new Booking
                {
                    SiteId = siteBSpace.SiteId,
                    ParkingSpaceId = siteBSpace.Id,
                    RequesterName = "Vendor Meeting",
                    PartyType = BookingPartyType.Visitor,
                    Company = "North East Supplies",
                    ContactNumber = "+267 555 0144",
                    StartDateTime = demoStart.AddHours(1),
                    EndDateTime = demoStart.AddHours(4),
                    IsPriority = false,
                    Status = BookingStatus.Active,
                    CreatedUtc = DateTime.UtcNow,
                    LastUpdatedUtc = DateTime.UtcNow,
                    CreatedByUserId = receptionist?.Id ?? string.Empty,
                    LastUpdatedByUserId = receptionist?.Id ?? string.Empty
                },
                new Booking
                {
                    SiteId = siteCSpace.SiteId,
                    ParkingSpaceId = siteCSpace.Id,
                    RequesterName = "Accessibility Support",
                    PartyType = BookingPartyType.Employee,
                    EmployeeNumber = "HR2044",
                    ContactNumber = "+267 555 0188",
                    StartDateTime = demoStart.AddHours(2),
                    EndDateTime = demoStart.AddHours(7),
                    IsPriority = false,
                    Status = BookingStatus.Active,
                    CreatedUtc = DateTime.UtcNow,
                    LastUpdatedUtc = DateTime.UtcNow,
                    CreatedByUserId = receptionist?.Id ?? string.Empty,
                    LastUpdatedByUserId = receptionist?.Id ?? string.Empty
                });

            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureUserAsync(UserManager<ApplicationUser> userManager, string role, string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, "LecturerDemo1!");
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"Could not create demo user '{email}'.");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }

    private static void CreateParkingSpaces(
        ApplicationDbContext context,
        Site site,
        int electricCount,
        int disabledCount,
        int visitorCount,
        int columns)
    {
        var parkingSpaces = new List<ParkingSpace>();
        for (var index = 0; index < site.TotalSpaces; index++)
        {
            var type = index < electricCount
                ? ParkingSpaceType.ElectricCharging
                : index < electricCount + disabledCount
                    ? ParkingSpaceType.Disabled
                    : index < electricCount + disabledCount + visitorCount
                        ? ParkingSpaceType.Visitor
                        : ParkingSpaceType.Standard;

            parkingSpaces.Add(new ParkingSpace
            {
                SiteId = site.Id,
                SpaceNumber = $"{site.Code}{index + 1:000}",
                Type = type,
                IsActive = true
            });
        }

        context.ParkingSpaces.AddRange(parkingSpaces);
        context.SaveChanges();

        context.LayoutPositions.AddRange(parkingSpaces.Select((space, index) => new LayoutPosition
        {
            ParkingSpaceId = space.Id,
            Row = (index / columns) + 1,
            Column = (index % columns) + 1
        }));
    }
}
