using TollCalculator.API.Constants;
using TollCalculator.API.Enums;
using TollCalculator.API.Extensions;
using TollCalculator.API.Models;

namespace TollCalculator.API.Services;

public class TollService : ITollService
{
    /// <summary>
    /// Calculates the total toll fee for a vehicle based on a series of dates and times it passed through toll stations.
    /// </summary>
    /// <param name="vehicleType">The type of vehicle for which the toll is being calculated.</param>
    /// <param name="dates">An array of DateTime objects representing the dates and times of toll station passages.</param>
    /// <returns>
    /// A TollResult object containing the total toll fee. If no dates are provided, it returns a message indicating that no dates were provided.
    /// If the vehicle type is toll-free, it returns a zero fee. The toll is calculated based on the highest toll amount within any given 60-minute period,
    /// without exceeding the maximum daily cap defined in TollConstants.MaxCost. Dates that fall on toll-free days or hours are excluded from the calculation.
    /// </returns>
    public TollResult CalculateToll(VehicleType vehicleType, DateTime[]? dates)
    {
        if (dates == null || !dates.Any())
        {
            return new TollResult("No dates provided.");
        }
    
        if (vehicleType.IsTollFree())
        {
            return new TollResult(0);
        }
    
        var tollFee = 0m;
        var highestTollInOneHour = 0m;
        DateTime? startOfHourWindow = null;
        
        foreach (var date in dates.OrderBy(d => d).Where(date => !date.IsTollFree()))
        {
            // If this is the first date or the date is outside the 60 minute window from the start of hour window
            if (!startOfHourWindow.HasValue || (date - startOfHourWindow.Value).TotalMinutes > 60)
            {
                // Add the highest toll fee of the previous hour to the total fee and reset
                tollFee += highestTollInOneHour;
                highestTollInOneHour = GetTollAmount(date);
                startOfHourWindow = date;
            }
            else
            {
                // If within the same 60-minute window, check if the current toll amount is higher
                var currentTollAmount = GetTollAmount(date);
                if (currentTollAmount > highestTollInOneHour)
                {
                    highestTollInOneHour = currentTollAmount;
                }
            }
            
            if (tollFee >= TollConstants.MaxCost)
            {
                return new TollResult(TollConstants.MaxCost);
            }
        }

        // Add the highest toll fee of the last hour to the total fee
        tollFee += highestTollInOneHour;
        return new TollResult(tollFee);
    }

    #region Private help methods
    private decimal GetTollAmount(DateTime passageTime)
    {
        if (passageTime.IsTollFree())
        {
            return 0;
        }

        //Should be cached and probably stored in a DB
        var allTimeSlots = GetTimeSlots();
        var matchingTimeSlot = allTimeSlots.FirstOrDefault(x => x.StartTime <= passageTime.TimeOfDay && x.EndTime >= passageTime.TimeOfDay);
        return matchingTimeSlot?.Cost ?? 0;
    }
    
    private static IEnumerable<TimeSlot> GetTimeSlots()
    {
        return new TimeSlot[]
        {
            new(new TimeSpan(06, 00, 0), new TimeSpan(06, 29, 59), 8),
            new(new TimeSpan(06, 30, 0), new TimeSpan(06, 59, 59), 13),
            new(new TimeSpan(07, 00, 0), new TimeSpan(07, 59, 59), 18),
            new(new TimeSpan(08, 00, 0), new TimeSpan(08, 29, 59), 13),
            new(new TimeSpan(08, 30, 0), new TimeSpan(14, 59, 59), 8),
            new(new TimeSpan(15, 00, 0), new TimeSpan(15, 29, 59), 13),
            new(new TimeSpan(15, 30, 0), new TimeSpan(16, 59, 59), 18),
            new(new TimeSpan(17, 00, 0), new TimeSpan(17, 59, 59), 13),
            new(new TimeSpan(18, 00, 0), new TimeSpan(18, 29, 59), 8)
        };
    }
    #endregion Private help methods
}