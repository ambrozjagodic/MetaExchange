using FluentAssertions;
using MetaExchange.Core;
using Moq;

namespace MetaExchange.Tests.Core
{
    public class ExchangeResultTest
    {
        [Theory]
        [InlineData(11, "")]
        [InlineData(99, null)]
        public void Constructor_SuccessResult_PropertiesSet(decimal totalPrice, string errorMsg)
        {
            IList<IOrderResult> orderResult = new List<IOrderResult> { Mock.Of<IOrderResult>() };

            IExchangeResult result = new ExchangeResult(orderResult, totalPrice, errorMsg);

            result.Id.Should().NotBeEmpty();
            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().Be(errorMsg);
            result.TotalPrice.Should().Be(totalPrice);
            result.OrderResult.Should().BeEquivalentTo(orderResult);
        }

        [Theory]
        [InlineData(11, "errorMsg1")]
        [InlineData(99, "errorMsg2")]
        public void Constructor_ErrorResult_PropertiesSet(decimal totalPrice, string errorMsg)
        {
            IExchangeResult result = new ExchangeResult(new List<IOrderResult>(), totalPrice, errorMsg);

            result.Id.Should().NotBeEmpty();
            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(errorMsg);
            result.TotalPrice.Should().Be(totalPrice);
            result.OrderResult.Should().BeEmpty();
        }
    }
}