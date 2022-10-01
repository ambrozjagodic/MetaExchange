using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.DataSource;
using MetaExchange.Logic;
using Moq;

namespace MetaExchange.Tests.Logic.MetaExchangeLogicTests
{
    public class SellOptimalTest : SellOptimalDriver
    {
        [Theory]
        [InlineData(15.26, 0.1, 18.2)] // first buyer leftover
        [InlineData(12.34, 0.2, 22.9)] // first buyer exactly the asked amount
        public async Task SellOptimal_FirstBuyerEnough_ReturnsFirstBuyerData(decimal amountBuyer1, decimal amountBuyer2, decimal amountBuyer3)
        {
            SetBuyerAmounts(amountBuyer1, amountBuyer2, amountBuyer3);

            IExchangeResult result = await Sut.SellOptimal(UserOrder);

            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(37020M);
            result.OrderResult.Should().HaveCount(1);
            result.OrderResult[0].Amount.Should().Be(12.34M);
            result.OrderResult[0].Exchange.Should().Be(Buyer1);
        }

        [Theory]
        [InlineData(2.8, 4.9, 20.3)] // third buyer leftover
        [InlineData(2.8, 4.9, 4.64)] // all three combined have exactly the asked amount
        public async Task SellOptimal_ThreeBuyersEnough_ReturnsThreeBuyersData(decimal amountBuyer1, decimal amountBuyer2, decimal amountBuyer3)
        {
            SetBuyerAmounts(amountBuyer1, amountBuyer2, amountBuyer3);

            IExchangeResult result = await Sut.SellOptimal(UserOrder);

            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(22840);
            result.OrderResult.Should().HaveCount(3);

            result.OrderResult[0].Amount.Should().Be(2.8M);
            result.OrderResult[0].Exchange.Should().Be(Buyer1);
            result.OrderResult[1].Amount.Should().Be(4.9M);
            result.OrderResult[1].Exchange.Should().Be(Buyer2);
            result.OrderResult[2].Amount.Should().Be(4.64M);
            result.OrderResult[2].Exchange.Should().Be(Buyer3);
        }

        [Fact]
        public async Task SellOptimal_NotEnoughBuyers_ReturnsErrorResult()
        {
            SetBuyerAmounts(0.1M, 0.2M, 0.3M);

            IExchangeResult result = await Sut.SellOptimal(UserOrder);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(NOT_ENOUGH_BUYERS);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }

        [Fact]
        public async Task SellOptimal_NoBuyers_ReturnsErrorResult()
        {
            SetNoBuyers();

            IExchangeResult result = await Sut.SellOptimal(UserOrder);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(NOT_ENOUGH_BUYERS);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }
    }

    public class SellOptimalDriver
    {
        public const string NOT_ENOUGH_BUYERS = "Not enough BTC buyers.";

        private readonly Mock<IMetaExchangeDataSource> _dataSource;

        public Bid Buyer1 { get; private set; }
        public Bid Buyer2 { get; private set; }
        public Bid Buyer3 { get; private set; }

        public IUserOrder UserOrder { get; }

        public IMetaExchangeLogic Sut { get; }

        public SellOptimalDriver()
        {
            _dataSource = new Mock<IMetaExchangeDataSource>();

            UserOrder = Mock.Of<IUserOrder>(i => i.Amount == 12.34M);

            Sut = new MetaExchangeLogic(null, null, _dataSource.Object);
        }

        public void SetBuyerAmounts(decimal amountBuyer1, decimal amountBuyer2, decimal amountBuyer3)
        {
            Buyer1 = Mock.Of<Bid>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountBuyer1 && j.Price == 3000));
            Buyer2 = Mock.Of<Bid>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountBuyer2 && j.Price == 2000));
            Buyer3 = Mock.Of<Bid>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountBuyer3 && j.Price == 1000));

            IList<Bid> buyers = new List<Bid>
            {
                Buyer1,
                Buyer2,
                Buyer3
            };

            _dataSource.Setup(i => i.GetOrderedBuyers()).ReturnsAsync(buyers);
        }

        public void SetNoBuyers()
        {
            _dataSource.Setup(i => i.GetOrderedBuyers()).ReturnsAsync(new List<Bid>());
        }
    }
}