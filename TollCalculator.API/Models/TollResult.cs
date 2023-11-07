using TollCalculator.API.Constants;

namespace TollCalculator.API.Models;

public class TollResult
{
    public string ErrorMessage { get; set; }
    public decimal TotalFee { get; set; }
    public bool Success { get; set; }
        
    public TollResult()
    {
        ErrorMessage = string.Empty;
    }

    public TollResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
        Success = false;
    }

    public TollResult(decimal tollFee)
    {
        ErrorMessage = string.Empty;
        TotalFee = Math.Min(tollFee, TollConstants.MaxCost);
        Success = true;
    }
}