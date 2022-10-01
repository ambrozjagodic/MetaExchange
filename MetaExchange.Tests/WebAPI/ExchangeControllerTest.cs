using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.Logic;
using MetaExchange.Validation;
using MetaExchange.WebAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MetaExchange.Tests.WebAPI
{
    public class ExchangeControllerTest : ExchangeControllerDriver
    {
        [Fact]
        public async Task GetOptimalTransactions_Buy_ReturnsBuyExchangeResult()
        {
            IActionResult result = await Sut.GetOptimalTransactions(UserOrderBuy);

            result.Should().BeOfType<CreatedResult>().Which.Value.Should().BeEquivalentTo(ExpectedBuyResult);
            result.Should().BeOfType<CreatedResult>().Which.Location.Should().BeEquivalentTo($"/{ExpectedBuyResult.Id}");
        }

        [Fact]
        public async Task GetOptimalTransactions_Sell_ReturnsSellExchangeResult()
        {
            IActionResult result = await Sut.GetOptimalTransactions(UserOrderSell);

            result.Should().BeOfType<CreatedResult>().Which.Value.Should().BeEquivalentTo(ExpectedSellResult);
            result.Should().BeOfType<CreatedResult>().Which.Location.Should().BeEquivalentTo($"/{ExpectedSellResult.Id}");
        }

        [Fact]
        public async Task GetOptimalTransactions_ValidationErrors_ReturnsBadRequestWithErrorMsg()
        {
            SetValidationErrors();

            IActionResult result = await Sut.GetOptimalTransactions(UserOrderBuy);

            result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(VALIDATION_ERROR_MSG);
            VerifyLogicNeverCalled();
        }

        [Fact]
        public async Task GetOptimalTransactions_Exception_ReturnsInternalServerError()
        {
            SetupLogicException();

            IActionResult result = await Sut.GetOptimalTransactions(UserOrderBuy);

            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            VerifyOutputStringCalled();
        }
    }

    public class ExchangeControllerDriver
    {
        public const string VALIDATION_ERROR_MSG = "SomeValidationErrorMsg";

        private readonly Mock<IMetaExchangeLogic> _logic;
        private readonly Mock<IWebAPIRequestValidation> _validation;
        private readonly Mock<IOutputWriter> _outputWriter;

        public UserOrder UserOrderBuy { get; }
        public UserOrder UserOrderSell { get; }
        public IExchangeResult ExpectedBuyResult { get; }
        public IExchangeResult ExpectedSellResult { get; }

        public ExchangeController Sut { get; }

        public ExchangeControllerDriver()
        {
            UserOrderBuy = new UserOrder { Type = "Buy" };
            UserOrderSell = new UserOrder { Type = "Sell" };

            ExpectedBuyResult = Mock.Of<IExchangeResult>(i => i.Id == Guid.NewGuid());
            ExpectedSellResult = Mock.Of<IExchangeResult>(i => i.Id == Guid.NewGuid());

            _validation = new Mock<IWebAPIRequestValidation>();
            _validation.Setup(i => i.ValidateUserOrder(UserOrderBuy)).Returns(string.Empty);
            _validation.Setup(i => i.ValidateUserOrder(UserOrderSell)).Returns(string.Empty);

            _logic = new Mock<IMetaExchangeLogic>();
            _logic.Setup(i => i.BuyOptimal(UserOrderBuy)).ReturnsAsync(ExpectedBuyResult);
            _logic.Setup(i => i.SellOptimal(UserOrderSell)).ReturnsAsync(ExpectedSellResult);

            _outputWriter = new Mock<IOutputWriter>();

            Sut = new ExchangeController(_logic.Object, _validation.Object, _outputWriter.Object);
        }

        public void SetValidationErrors()
        {
            _validation.Setup(i => i.ValidateUserOrder(UserOrderBuy)).Returns(VALIDATION_ERROR_MSG);
        }

        public void SetupLogicException()
        {
            _logic.Setup(i => i.BuyOptimal(UserOrderBuy)).ThrowsAsync(new Exception("test"));
        }

        public void VerifyLogicNeverCalled()
        {
            _logic.VerifyNoOtherCalls();
        }

        public void VerifyOutputStringCalled()
        {
            _outputWriter.Verify(i => i.OutputString(It.IsAny<string>()), Times.Once);
        }
    }
}