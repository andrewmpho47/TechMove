namespace TechMove.Services
{
    public class CurrencyCalculator
    {
        public decimal ConvertUsdToZar(decimal usdAmount, decimal zarRate)
        {
            return usdAmount * zarRate;
        }
    }
}