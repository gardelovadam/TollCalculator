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
        
        var totalTollFee = 0m;
        
        var groupedDates = dates
            .Where(date => !date.IsTollFree())
            .OrderBy(d => d)
            .GroupBy(date => date.Date);
    
        foreach (var dailyDates in groupedDates)
        {
            var dailyTollFee = 0m;
            var highestTollInOneHour = 0m;
            DateTime? startOfHourWindow = null;

            foreach (var date in dailyDates)
            {
                if (!startOfHourWindow.HasValue || (date - startOfHourWindow.Value).TotalMinutes > 60)
                {
                    dailyTollFee += highestTollInOneHour;
                    if (dailyTollFee >= TollConstants.MaxCost)
                    {
                        dailyTollFee = TollConstants.MaxCost;
                        break;
                    }
                    highestTollInOneHour = GetTollAmount(date);
                    startOfHourWindow = date;
                }
                else
                {
                    var currentTollAmount = GetTollAmount(date);
                    if (currentTollAmount > highestTollInOneHour)
                    {
                        highestTollInOneHour = currentTollAmount;
                    }
                }
            }

            dailyTollFee += highestTollInOneHour;
            if (dailyTollFee > TollConstants.MaxCost)
            {
                dailyTollFee = TollConstants.MaxCost;
            }

            totalTollFee += dailyTollFee;
        }

        return new TollResult(totalTollFee);
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