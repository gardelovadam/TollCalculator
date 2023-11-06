namespace TollCalculator.API.Models;

public class TimeSlot
{
    public int Id { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal Cost { get; set; }

    public TimeSlot() { }

    public TimeSlot(int id, TimeSpan startTime, TimeSpan endTime, decimal cost)
    {
        Id = id;
        StartTime = startTime;
        EndTime = endTime;
        Cost = cost;
    }
}