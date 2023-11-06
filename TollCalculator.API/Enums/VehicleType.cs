using System.Text.Json.Serialization;

namespace TollCalculator.API.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VehicleType
{
    Car = 0,
    Motorbike = 1,
    //Toll free
    Tractor = 2, 
    Emergency = 3,
    Diplomat = 4,
    Foreign = 5,
    Military = 6
}