using TollCalculator.API.Enums;

namespace TollCalculator.API.Extensions;

public static class VehicleTypeExtensions
{
    /// <summary>
    /// Checks if the vehicle is toll-free.
    /// </summary>
    /// <param name="type">The vehicle type to check.</param>
    /// <returns>True if the vehicle is toll-free, false otherwise.</returns>
    public static bool IsTollFree(this VehicleType type)
    {
        return type != VehicleType.Car && type != VehicleType.Motorbike;
    }
}