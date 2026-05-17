using System.Collections.Generic;

namespace CarparkManagementSystem.ViewModels.Home;

public class ManagerDashboardViewModel
{
    public string CurrentRole { get; init; } = "Facilities Manager";

    public int PriorityBookingCount { get; init; }

    public int SitesWithLowAvailability { get; init; }

     public int    UnreadNotificationCount  { get; set; } 

    public List<SiteSummaryCardViewModel> Sites { get; init; } = [];

    public List<DashboardBookingViewModel> UpcomingPriorityBookings { get; init; } = [];

    public List<ManagerBookingRowViewModel> AllBookings             { get; set; } = [];
}
