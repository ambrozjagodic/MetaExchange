using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.DataSource;
using MetaExchange.Logic;
using Moq;

namespace MetaExchange.Tests.Logic.MetaExchangeLogicTests
{
    public class BuyOptimalTest : BuyOptimalDriver
    {
        [Theory]
        [InlineData(15.26, 0.1, 18.2)] // first seller leftover
        [InlineData(12.34, 0.2, 22.9)] // first seller exactly the asked amount
        public async Task BuyOptimal_FirstSellerHasEnough_ReturnsFirstSellerData(decimal amountSeller1, decimal amountSeller2, decimal amountSeller3)
        {
            SetSellerAmounts(amountSeller1, amountSeller2, amountSeller3);

            IExchangeResult result = await Sut.BuyOptimal(UserOrder);

            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(12340);
            result.OrderResult.Should().HaveCount(1);
            result.OrderResult[0].Amount.Should().Be(12.34M);
            result.OrderResult[0].Exchange.Should().Be(Asker1);
        }

        [Theory]
        [InlineData(2.8, 4.9, 20.3)] // third seller leftover
        [InlineData(2.8, 4.9, 4.64)] // all three combined have exactly the asked amount
        public async Task BuyOptimal_ThreeSellersHaveEnough_ReturnsThreeSellersData(decimal amountSeller1, decimal amountSeller2, decimal amountSeller3)
        {
            SetSellerAmounts(amountSeller1, amountSeller2, amountSeller3);

            IExchangeResult result = await Sut.BuyOptimal(UserOrder);

            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(12354.18M);
            result.OrderResult.Should().HaveCount(3);

            result.OrderResult[0].Amount.Should().Be(2.8M);
            result.OrderResult[0].Exchange.Should().Be(Asker1);
            result.OrderResult[1].Amount.Should().Be(4.9M);
            result.OrderResult[1].Exchange.Should().Be(Asker2);
            result.OrderResult[2].Amount.Should().Be(4.64M);
            result.OrderResult[2].Exchange.Should().Be(Asker3);
        }

        [Theory]
        [InlineData(2.8)] // less than buy amount
        [InlineData(40.92)] // more than buy amount
        public async Task BuyOptimal_BudgetExceededOnFirstSeller_ReturnsErrorResult(decimal amountSeller1)
        {
            SetBudgetExceededOnFirstSeller(amountSeller1);

            IExchangeResult result = await Sut.BuyOptimal(UserOrder);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(BUDGET_EXCEEDED_ERROR_MSG);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }

        [Fact]
        public async Task BuyOptimal_BudgetExceededOnThirdSeller_ReturnsErrorResult()
        {
            SetBudgetExceededOnThirdSeller(1, 2, 40);

            IExchangeResult result = await Sut.BuyOptimal(UserOrder);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(BUDGET_EXCEEDED_ERROR_MSG);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }

        [Fact]
        public async Task BuyOptimal_NotEnoughBTCToBuy_ReturnsErrorResult()
        {
            SetSellerAmounts(0.1M, 0.2M, 0.3M);

            IExchangeResult result = await Sut.BuyOptimal(UserOrder);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(NOT_ENOUGH_BTC_TO_BUY);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }

        [Fact]
        public async Task BuyOptimal_NoSellers_ReturnsErrorResult()
        {
            SetNoSellers();

            IExchangeResult result = await Sut.BuyOptimal(UserOrder);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(NOT_ENOUGH_BTC_TO_BUY);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }
    }

    public class BuyOptimalDriver
    {
        public const string BUDGET_EXCEEDED_ERROR_MSG = "Budget exceeded.";
        public const string NOT_ENOUGH_BTC_TO_BUY = "Not enough BTC to buy.";

        private readonly Mock<ISequenceFinder> _sequenceFinder;
        private readonly Mock<IOutputWriter> _outputWriter;
        private readonly Mock<IMetaExchangeDataSource> _dataSource;

        public Ask Asker1 { get; private set; }
        public Ask Asker2 { get; private set; }
        public Ask Asker3 { get; private set; }

        public IUserOrder UserOrder { get; }

        public IMetaExchangeLogic Sut { get; }

        public BuyOptimalDriver()
        {
            _sequenceFinder = new Mock<ISequenceFinder>();
            _outputWriter = new Mock<IOutputWriter>();
            _dataSource = new Mock<IMetaExchangeDataSource>();

            UserOrder = Mock.Of<IUserOrder>(i => i.Amount == 12.34M && i.BalanceEur == 40000);

            Sut = new MetaExchangeLogic(_sequenceFinder.Object, _outputWriter.Object, _dataSource.Object);
        }

        public void SetSellerAmounts(decimal amountSeller1, decimal amountSeller2, decimal amountSeller3)
        {
            Asker1 = Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller1 && j.Price == 1000));
            Asker2 = Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller2 && j.Price == 1001));
            Asker3 = Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller3 && j.Price == 1002));

            IList<Ask> sellers = new List<Ask>
            {
                Asker1,
                Asker2,
                Asker3
            };

            _dataSource.Setup(i => i.GetOrderedSellers()).ReturnsAsync(sellers);
        }

        public void SetBudgetExceededOnFirstSeller(decimal amountSeller1)
        {
            Asker1 = Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller1 && j.Price == 99999));
            Asker2 = Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == 15.321M && j.Price == 1001));
            Asker3 = Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == 30.123M && j.Price == 1002));

            IList<Ask> sellers = new List<Ask>
            {
                Asker1,
                Asker2,
                Asker3
            };

            _dataSource.Setup(i => i.GetOrderedSellers()).ReturnsAsync(sellers);
        }

        public void SetBudgetExceededOnThirdSeller(decimal amountSeller1, decimal amountSeller2, decimal amountSeller3)
        {
            Asker1 = Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller1 && j.Price == 10000));
            Asker2 = Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller2 && j.Price == 15000));
            Asker3 = Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller3 && j.Price == 30000));

            IList<Ask> sellers = new List<Ask>
            {
                Asker1,
                Asker2,
                Asker3
            };

            _dataSource.Setup(i => i.GetOrderedSellers()).ReturnsAsync(sellers);
        }

        public void SetNoSellers()
        {
            _dataSource.Setup(i => i.GetOrderedSellers()).ReturnsAsync(new List<Ask>());
        }
    }
}