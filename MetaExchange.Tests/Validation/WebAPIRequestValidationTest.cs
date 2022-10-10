using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.Validation;
using Moq;

namespace MetaExchange.Tests.Validation
{
    public class WebAPIRequestValidationTest : WebAPIRequestValidationDriver
    {
        [Fact]
        public void ValidateUserOrder_Null_ErrorMessageReturned()
        {
            string result = Sut.ValidateUserOrder(null);

            result.Should().Be("Invalid object received (null).");
        }

        [Theory]
        [InlineData("")]
        [InlineData("asd")]
        public void ValidateUserOrder_InvalidOrderType_ErrorMessageReturned(string orderType)
        {
            IUserOrder userOrder = Mock.Of<IUserOrder>(i => i.Type == orderType);

            string result = Sut.ValidateUserOrder(userOrder);

            result.Should().Be("Invalid order type received.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-20)]
        public void ValidateUserOrder_InvalidAmount_ErrorMessageReturned(decimal amount)
        {
            IUserOrder userOrder = Mock.Of<IUserOrder>(i => i.Type == "buy" && i.Amount == amount);

            string result = Sut.ValidateUserOrder(userOrder);

            result.Should().Be("Invalid exchange amount received.");
        }

        [Fact]
        public void ValidateUserOrder_ValidOrder_ReturnsEmptyString()
        {
            IUserOrder userOrder = Mock.Of<IUserOrder>(i => i.Type == "buy" && i.Amount == 10);

            string result = Sut.ValidateUserOrder(userOrder);

            result.Should().BeEmpty();
        }
    }

    public class WebAPIRequestValidationDriver
    {
        public IWebAPIRequestValidation Sut { get; }

        public WebAPIRequestValidationDriver()
        {
            Sut = new WebAPIRequestValidation();
        }
    }
}