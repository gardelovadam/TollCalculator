using TollCalculator.API.Constants;
using TollCalculator.API.Enums;
using TollCalculator.API.Services;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace TollCalculator.Tests;

[TestFixture]
public class TollServiceTests
{
    private TollService _tollService;

    [SetUp]
    public void Setup()
    {
        _tollService = new TollService();
    }

    [Test]
    public void CalculateToll_NoDatesProvided_ReturnsErrorMessage()
    {
        var result = _tollService.CalculateToll(VehicleType.Car, null);
        Assert.That(result.ErrorMessage, Is.EqualTo("No dates provided."));
    }
    
    [TestCase(VehicleType.Tractor)]
    [TestCase(VehicleType.Emergency)]
    [TestCase(VehicleType.Diplomat)]
    [TestCase(VehicleType.Foreign)]
    [TestCase(VehicleType.Military)]
    public void CalculateToll_TollFreeVehicle_ReturnsZeroToll(VehicleType vehicle)
    {
        var result = _tollService.CalculateToll(vehicle, new DateTime[] { DateTime.Now });
        Assert.That(result.TotalFee, Is.EqualTo(0));
    }
    
    [Test]
    public void CalculateToll_SingleDateWithToll_ReturnsCorrectToll()
    {
        var passageTime = new DateTime(2023, 11, 6, 7, 0, 0);

        var result = _tollService.CalculateToll(VehicleType.Car, new[] { passageTime });
        Assert.That(result.TotalFee, Is.EqualTo(18m));
    }

    [Test]
    public void CalculateToll_MultipleDatesWithinOneHour_ReturnsHighestTollForHour()
    {
        var dateTimes = new[]
        {
            new DateTime(2023, 11, 6, 6, 19, 0),
            new DateTime(2023, 11, 6, 6, 39, 0),
            new DateTime(2023, 11, 6, 7, 22, 0),
        };

        var result = _tollService.CalculateToll(VehicleType.Car, dateTimes);
        Assert.That(result.TotalFee, Is.EqualTo(31m));
    }

    [Test]
    public void CalculateToll_ShouldCalculateCorrectToll_ForMultipleDatesWithOwnHourlyLimit()
    {
        var dateTimes = new[]
        {
            //Day 1
            new DateTime(2023, 11, 6, 6, 19, 0),
            new DateTime(2023, 11, 6, 6, 39, 0),
            new DateTime(2023, 11, 6, 7, 22, 0),
            //Day 2
            new DateTime(2023, 11, 7, 6, 19, 0),
            new DateTime(2023, 11, 7, 6, 39, 0),
        };

        var result = _tollService.CalculateToll(VehicleType.Car, dateTimes);
        
        // Assert
        Assert.That(result.TotalFee, Is.EqualTo(44));
    }

    [Test]
    public void CalculateToll_MultipleDatesExceedingMaxCap_ReturnsMaxCap()
    {
        var dateTimes = new[]
        {
            new DateTime(2023, 11, 6, 7, 0, 0),
            new DateTime(2023, 11, 6, 8, 0, 0),
            new DateTime(2023, 11, 6, 15, 0, 0),
            new DateTime(2023, 11, 6, 16, 0, 0),
            new DateTime(2023, 11, 6, 17, 0, 0),
            new DateTime(2023, 11, 6, 7, 30, 0),
            new DateTime(2023, 11, 6, 8, 30, 0),
            new DateTime(2023, 11, 6, 16, 30, 0) 
        };
    
        var result = _tollService.CalculateToll(VehicleType.Car, dateTimes);
        Assert.That(result.TotalFee, Is.EqualTo(TollConstants.MaxCost));
    }
}