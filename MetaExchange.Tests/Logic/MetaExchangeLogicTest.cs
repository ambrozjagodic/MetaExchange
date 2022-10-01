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
        public async Task BuyOptimal_UserOrder_ReturnsExhcangeResult()
        {
            IExchangeResult result = await Sut.BuyOptimal(BuyOrder);

            result.Should().Be(ExpectedBuyResult);
        }

        [Fact]
        public async Task SellOptimal_UserOrder_ReturnsExhcangeResult()
        {
            IExchangeResult result = await Sut.SellOptimal(SellOrder);

            result.Should().Be(ExpectedSellResult);
        }

        [Fact]
        public void FindOptimalSequencePerExchange_TwoOrderBooks_ProcessTwoBooks()
        {
            Sut.FindOptimalSequencePerExchange(NumberOfOrderBooks);

            VerifyInitialValuesOutput();
            VerifyResultOutput();
        }
    }

    public class MetaExchangeLogicDriver
    {
        private readonly Mock<ISequenceFinder> _sequenceFinder;
        private readonly Mock<IOutputWriter> _outputWriter;
        private readonly Mock<IMetaExchangeDataSource> _dataSource;

        private readonly IExchangeResult _buyResult1;
        private readonly IExchangeResult _buyResult2;
        private readonly IExchangeResult _sellResult1;
        private readonly IExchangeResult _sellResult2;

        public IUserOrder BuyOrder { get; }
        public IUserOrder SellOrder { get; }

        public IExchangeResult ExpectedBuyResult { get; }
        public IExchangeResult ExpectedSellResult { get; }

        public int NumberOfOrderBooks { get; }

        public IMetaExchangeLogic Sut { get; }

        public MetaExchangeLogicDriver()
        {
            NumberOfOrderBooks = 2;
            BuyOrder = Mock.Of<IUserOrder>(i => i.Amount == 10 && i.BalanceEur == 1000);
            SellOrder = Mock.Of<IUserOrder>(i => i.Amount == 20 && i.BalanceBTC == 2000);
            ExpectedBuyResult = Mock.Of<IExchangeResult>();
            ExpectedSellResult = Mock.Of<IExchangeResult>();

            IList<Ask> sellers = new List<Ask> { new Ask(), new Ask() };
            IList<Bid> buyers = new List<Bid> { new Bid(), new Bid() };
            _dataSource = new Mock<IMetaExchangeDataSource>();
            _dataSource.Setup(i => i.GetOrderedSellers()).ReturnsAsync(sellers);
            _dataSource.Setup(i => i.GetOrderedBuyers()).ReturnsAsync(buyers);

            _sequenceFinder = new Mock<ISequenceFinder>();
            _sequenceFinder.Setup(i => i.FindOptimalBuySequence(10, 1000, sellers)).Returns(ExpectedBuyResult);
            _sequenceFinder.Setup(i => i.FindOptimalSellSequence(20, 2000, buyers)).Returns(ExpectedSellResult);

            _outputWriter = new Mock<IOutputWriter>();

            List<Bid> bids1 = new() { new Bid(), new Bid() };
            List<Bid> bids2 = new() { new Bid(), new Bid() };
            List<Ask> asks1 = new() { new Ask(), new Ask() };
            List<Ask> asks2 = new() { new Ask(), new Ask() };
            IList<OrderBook> orderBooks = new List<OrderBook>
            {
                new OrderBook("2022-01-01T00:00:00", bids1, asks1),
                new OrderBook("2022-01-01T00:00:00", bids2, asks2)
            };

            _dataSource.Setup(i => i.GetLastNumberOfOrderBooks(NumberOfOrderBooks)).Returns(orderBooks);

            _buyResult1 = Mock.Of<IExchangeResult>();
            _buyResult2 = Mock.Of<IExchangeResult>();
            _sellResult1 = Mock.Of<IExchangeResult>();
            _sellResult2 = Mock.Of<IExchangeResult>();
            _sequenceFinder.Setup(i => i.FindOptimalBuySequence(It.IsAny<decimal>(), It.IsAny<decimal>(), asks1)).Returns(_buyResult1);
            _sequenceFinder.Setup(i => i.FindOptimalBuySequence(It.IsAny<decimal>(), It.IsAny<decimal>(), asks2)).Returns(_buyResult2);
            _sequenceFinder.Setup(i => i.FindOptimalSellSequence(It.IsAny<decimal>(), It.IsAny<decimal>(), bids1)).Returns(_sellResult1);
            _sequenceFinder.Setup(i => i.FindOptimalSellSequence(It.IsAny<decimal>(), It.IsAny<decimal>(), bids2)).Returns(_sellResult2);

            Sut = new MetaExchangeLogic(_sequenceFinder.Object, _outputWriter.Object, _dataSource.Object);
        }

        public void VerifyInitialValuesOutput()
        {
            _outputWriter.Verify(i => i.OutputInitialValues(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Exactly(2));
        }

        public void VerifyResultOutput()
        {
            _outputWriter.Verify(i => i.OutputResultSequence(_buyResult1, _sellResult1), Times.Once);
            _outputWriter.Verify(i => i.OutputResultSequence(_buyResult2, _sellResult2), Times.Once);
        }
    }
}