using FluentAssertions;
using MetaExchange.Core;
using Moq;

namespace MetaExchange.Tests.Core
{
    public class OrderResultTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Constructor_Called_PropertiesSet(decimal amount)
        {
            IExchange exchange = Mock.Of<IExchange>();

            IOrderResult result = new OrderResult(amount, exchange);

            result.Amount.Should().Be(amount);
            result.Exchange.Should().Be(exchange);
        }
    }
}