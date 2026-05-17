namespace CarparkManagementSystem.ViewModels.Home;

public class AllBookingsManagerViewModel
{
    public List<ManagerBookingRowViewModel> Bookings { get; set; } = [];
    public int UnreadNotificationCount { get; set; }
}

public class ManagerBookingRowViewModel
{
    public int Id { get; set; }
    public string SiteName        { get; set; } = string.Empty;
    public string SpaceNumber     { get; set; } = string.Empty;
    public string RequesterName   { get; set; } = string.Empty;
    public string PartyTypeLabel  { get; set; } = string.Empty;
    public string StartDisplay    { get; set; } = string.Empty;
    public string EndDisplay      { get; set; } = string.Empty;
    public bool   IsPriority      { get; set; }
    public string StatusLabel     { get; set; } = string.Empty;
    public string StatusClass     { get; set; } = string.Empty;
    public string CreatedByName   { get; set; } = string.Empty;
    public string CreatedByRole   { get; set; } = string.Empty;
    public bool   CanOverride     { get; set; }  // false if already priority/cancelled/overridden
    public string? OverriddenRequesterName { get; set; }
    public bool    IsOverrideReplacement   { get; set; }
}