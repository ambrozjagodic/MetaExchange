using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.DataSource;
using MetaExchange.Logic;
using Moq;

namespace MetaExchange.Tests.Logic
{
    public class MetaExchangeLogicTest : MetaExchangeLogicDriver
    {
        [Fact]
        public async Task BuyOptimal_ValidSellerData_ReturnsExhcangeResult()
        {
            IExchangeResult result = await Sut.BuyOptimal(BuyOrder);

            result.Should().Be(ExpectedBuyResult);
        }

        [Fact]
        public async Task BuyOptimal_NullSellerData_ReturnsErrorResult()
        {
            SetNullData();

            IExchangeResult result = await Sut.BuyOptimal(BuyOrder);

            result.Should().BeEquivalentTo(ExpectedErrorResult, i => i.Excluding(j => j.Id));
        }

        [Fact]
        public async Task SellOptimal_ValidBuyerData_ReturnsExhcangeResult()
        {
            IExchangeResult result = await Sut.SellOptimal(SellOrder);

            result.Should().Be(ExpectedSellResult);
        }

        [Fact]
        public async Task SellOptimal_NullBuyerData_ReturnsErrorResult()
        {
            SetNullData();

            IExchangeResult result = await Sut.SellOptimal(SellOrder);

            result.Should().BeEquivalentTo(ExpectedErrorResult, i => i.Excluding(j => j.Id));
        }
    }

    public class MetaExchangeLogicDriver
    {
        private readonly Mock<ISequenceFinder> _sequenceFinder;
        private readonly Mock<IMetaExchangeDataSource> _dataSource;

        public IUserOrder BuyOrder { get; }
        public IUserOrder SellOrder { get; }

        public IExchangeResult ExpectedBuyResult { get; }
        public IExchangeResult ExpectedSellResult { get; }
        public IExchangeResult ExpectedErrorResult { get; }

        public IMetaExchangeLogic Sut { get; }

        public MetaExchangeLogicDriver()
        {
            BuyOrder = Mock.Of<IUserOrder>(i => i.Amount == 10);
            SellOrder = Mock.Of<IUserOrder>(i => i.Amount == 20);
            ExpectedBuyResult = Mock.Of<IExchangeResult>();
            ExpectedSellResult = Mock.Of<IExchangeResult>();
            ExpectedErrorResult = Mock.Of<IExchangeResult>(i => !i.Success && i.OrderResult == new List<IOrderResult>() && i.TotalPrice == 0 && i.ErrorMsg == Consts.ORDER_BOOK_ERROR);

            IOrderBookSellerData sellerData = Mock.Of<IOrderBookSellerData>();
            IOrderBookBuyerData buyerData = Mock.Of<IOrderBookBuyerData>();
            _dataSource = new Mock<IMetaExchangeDataSource>();
            _dataSource.Setup(i => i.GetSellersData()).ReturnsAsync(sellerData);
            _dataSource.Setup(i => i.GetBuyersData()).ReturnsAsync(buyerData);

            _sequenceFinder = new Mock<ISequenceFinder>();
            _sequenceFinder.Setup(i => i.FindOptimalBuySequence(10, sellerData)).Returns(ExpectedBuyResult);
            _sequenceFinder.Setup(i => i.FindOptimalSellSequence(20, buyerData)).Returns(ExpectedSellResult);

            Sut = new MetaExchangeLogic(_sequenceFinder.Object, _dataSource.Object);
        }

        public void SetNullData()
        {
            _dataSource.Setup(i => i.GetSellersData()).ReturnsAsync(() => null);
            _dataSource.Setup(i => i.GetBuyersData()).ReturnsAsync(() => null);
        }
    }
}