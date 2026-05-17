using System.Collections.Generic;

namespace CarparkManagementSystem.ViewModels.Home;

public class UserDashboardViewModel
{
    public string CurrentRole { get; init; } = "User";

    public int SiteCount { get; init; }

    public int TotalSpaceCount { get; init; }

    public int AvailableSpaceCount { get; init; }

    public int UpcomingBookingCount { get; init; }

    public int PastBookingCount { get; init; }

    public List<DashboardBookingViewModel> MyBookings { get; init; } = [];
}
