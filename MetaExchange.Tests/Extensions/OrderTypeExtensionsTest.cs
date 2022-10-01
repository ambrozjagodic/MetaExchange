using FluentAssertions;
using MetaExchange.Extensions;

namespace MetaExchange.Tests.Extensions
{
    public class OrderTypeExtensionsTest
    {
        [Theory]
        [InlineData("BUY")]
        [InlineData("buy")]
        [InlineData("bUy")]
        public void IsBuying_Yes_ReturnsTrue(string orderType)
        {
            bool result = orderType.IsBuying();

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("Sell")]
        [InlineData("something")]
        public void IsBuying_No_ReturnsFalse(string orderType)
        {
            bool result = orderType.IsBuying();

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("SELL")]
        [InlineData("sell")]
        [InlineData("SelL")]
        public void IsSelling_Yes_ReturnsTrue(string orderType)
        {
            bool result = orderType.IsSelling();

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("Buy")]
        [InlineData("something")]
        public void IsSelling_No_ReturnsFalse(string orderType)
        {
            bool result = orderType.IsSelling();

            result.Should().BeFalse();
        }
    }
}