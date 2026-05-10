using TechMove.Services;

namespace TechMove.Tests
{
    public class CurrencyCalculatorTests
    {
        [Fact]
        public void ConvertUsdToZar_ShouldReturnCorrectAmount_WhenRateIsValid()
        {
            var calculator = new CurrencyCalculator();

            var result = calculator.ConvertUsdToZar(100, 18.50m);

            Assert.Equal(1850m, result);
        }

        [Fact]
        public void ConvertUsdToZar_ShouldReturnZero_WhenUsdAmountIsZero()
        {
            var calculator = new CurrencyCalculator();

            var result = calculator.ConvertUsdToZar(0, 18.50m);

            Assert.Equal(0m, result);
        }
    }
}