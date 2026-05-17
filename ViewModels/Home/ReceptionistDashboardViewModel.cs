using System.Collections.Generic;

namespace CarparkManagementSystem.ViewModels.Home;

public class ReceptionistDashboardViewModel
{
    public string CurrentRole { get; init; } = "Receptionist/Admin";

    public int VisitorBookingsToday { get; init; }

    public int OpenVisitorBookings { get; init; }

    public int AvailableVisitorSpaces { get; init; }

    public int UnreadNotificationCount { get; set; }

    public List<SiteSummaryCardViewModel> Sites { get; init; } = [];

    public List<DashboardBookingViewModel> UpcomingVisitorBookings { get; init; } = [];
}
