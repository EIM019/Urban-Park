using System.Security.Claims;
using CarparkManagementSystem.Models;
using CarparkManagementSystem.ViewModels.Availability;
using CarparkManagementSystem.ViewModels.Bookings;
using CarparkManagementSystem.ViewModels.Home;

namespace CarparkManagementSystem.Services;

public interface IBookingService
{
    Task<HomeDashboardViewModel> GetHomeDashboardAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);

    Task<UserDashboardViewModel> GetUserDashboardAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);

    Task<ReceptionistDashboardViewModel> GetReceptionistDashboardAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);

    Task<ManagerDashboardViewModel> GetManagerDashboardAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);

    Task<ITDashboardViewModel> GetITDashboardAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);

    
    Task<ServiceResult> OverrideBookingAsync(OverrideBookingViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task<AllBookingsManagerViewModel> GetAllBookingsForManagerAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task<OverrideBookingViewModel?> BuildOverrideFormAsync(int bookingId, ClaimsPrincipal user, CancellationToken cancellationToken = default);

    Task<AvailabilitySearchViewModel> BuildAvailabilitySearchAsync(
        int? siteId,
        DateTime? startDateTime,
        DateTime? endDateTime,
        ParkingSpaceType? spaceType,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    Task<BookingsListViewModel> GetBookingsAsync(
        int? siteId,
        string? statusFilter,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    Task<BookingFormViewModel?> GetCreateBookingAsync(
        int siteId,
        int parkingSpaceId,
        DateTime startDateTime,
        DateTime endDateTime,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    Task<BookingFormViewModel?> GetEditBookingAsync(
        int id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    Task PopulateBookingFormAsync(BookingFormViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default);

    Task<ServiceResult> CreateBookingAsync(BookingFormViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default);

    Task<ServiceResult> UpdateBookingAsync(BookingFormViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default);

    Task<ServiceResult> CancelBookingAsync(int id, ClaimsPrincipal user, CancellationToken cancellationToken = default);
}
