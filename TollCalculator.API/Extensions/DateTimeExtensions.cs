namespace TollCalculator.API.Extensions;

public static class DateTimeExtensions
{
    //Public holidays for 2023
    private static readonly HashSet<DateTime> PublicHolidays = new()
    {
        new DateTime(2023, 1, 1),
        new DateTime(2023, 1, 6),
        new DateTime(2023, 4, 7),
        new DateTime(2023, 4, 9),
        new DateTime(2023, 4, 10),
        new DateTime(2023, 5, 1),
        new DateTime(2023, 5, 18),
        new DateTime(2023, 6, 6),
        new DateTime(2023, 6, 24),
        new DateTime(2023, 11, 4),
        new DateTime(2023, 12, 24),
        new DateTime(2023, 12, 25),
        new DateTime(2023, 12, 26),
        new DateTime(2023, 12, 31),
    };
    
    /// <summary>
    /// Checks if the date is toll-free.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if the date is toll-free, false otherwise.</returns>
    public static bool IsTollFree(this DateTime date)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return true;
        }
        
        if (date.Month == 7)
        {
            return true;
        }
        
        if (PublicHolidays.Contains(date.Date))
        {
            return true;
        }
        
        if (PublicHolidays.Contains(date.Date.AddDays(1)))
        {
            return true;
        }
        
        return false;
    }
}