namespace TollCalculator.API.Models;

public class TimeSlot
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal Cost { get; set; }

    public TimeSlot(TimeSpan startTime, TimeSpan endTime, decimal cost)
    {
        StartTime = startTime;
        EndTime = endTime;
        Cost = cost;
    }
}