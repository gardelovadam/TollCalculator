using TollCalculator.API.Enums;
using TollCalculator.API.Models;

namespace TollCalculator.API.Services;

public interface ITollService
{
    TollResult CalculateToll(VehicleType vehicleType, DateTime[]? dates);
}