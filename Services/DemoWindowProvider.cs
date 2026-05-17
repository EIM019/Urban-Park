namespace CarparkManagementSystem.Services;

public static class DemoWindowProvider
{
    public static (DateTime Start, DateTime End) GetDefaultWindow()
    {
        var day = DateTime.Today.AddDays(1);

        while (day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            day = day.AddDays(1);
        }

        return (day.AddHours(8), day.AddHours(17));
    }
}
