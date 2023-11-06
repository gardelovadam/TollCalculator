using Microsoft.AspNetCore.Mvc;
using TollCalculator.API.Enums;
using TollCalculator.API.Models;
using TollCalculator.API.Services;

namespace TollCalculator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TollCalculationController : ControllerBase
{
    private readonly ITollService _tollService;
    public TollCalculationController(ITollService tollService)
    {
        _tollService = tollService;
    }

    /// <summary>
    /// Calculates the toll fee for a vehicle based on its type and dates.
    /// </summary>
    /// <param name="vehicleType">The type of the vehicle.</param>
    /// <param name="dates">The dates for which to calculate the toll fee.</param>
    /// <returns>The calculated toll fee.</returns>
    [ProducesResponseType(typeof(TollResult), 200)]
    [ProducesResponseType(typeof(TollResult), 400)]
    [HttpGet("GetTollFee")]
    public ActionResult<TollResult> GetTollFee(VehicleType vehicleType, [FromQuery] DateTime[] dates)
    {
        var result = _tollService.CalculateToll(vehicleType, dates);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        
        return Ok(result);
    }
}