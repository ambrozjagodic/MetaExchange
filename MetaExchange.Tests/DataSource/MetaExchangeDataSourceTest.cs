using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.DataSource;
using MetaExchange.Logic;
using Moq;

namespace MetaExchange.Tests.DataSource
{
    public class MetaExchangeDataSourceTest : MetaExchangeDataSourceDriver
    {
        [Fact]
        public async Task GetBuyersData_Success_InitializedAndBuyerDataReturned()
        {
            IOrderBookBuyerData result = await Sut.GetBuyersData();

            result.Should().Be(ExpectedBuyerData);
        }

        [Fact]
        public async Task GetBuyersData_AlreadyInitialized_InitCalledOnceAndBuyerDataReturned()
        {
            await Sut.Init();

            IOrderBookBuyerData result = await Sut.GetBuyersData();

            result.Should().Be(ExpectedBuyerData);
        }

        [Fact]
        public async Task GetBuyersData_InitException_WriteOutputAndReturnNull()
        {
            SetInitException();

            IOrderBookBuyerData result = await Sut.GetBuyersData();

            result.Should().BeNull();
            VerifyOutputWriterCalled();
        }

        [Fact]
        public async Task GetSellersData_Success_InitializedAndSellerDataReturned()
        {
            IOrderBookSellerData result = await Sut.GetSellersData();

            result.Should().Be(ExpectedSellerData);
        }

        [Fact]
        public async Task GetSellersData_AlreadyInitialized_InitCalledOnceAndSellerDataReturned()
        {
            await Sut.Init();

            IOrderBookSellerData result = await Sut.GetSellersData();

            result.Should().Be(ExpectedSellerData);
        }

        [Fact]
        public async Task GetSellersData_InitException_WriteOutputAndReturnNull()
        {
            SetInitException();

            IOrderBookSellerData result = await Sut.GetSellersData();

            result.Should().BeNull();
            VerifyOutputWriterCalled();
        }
    }

    public class MetaExchangeDataSourceDriver
    {
        private readonly Mock<IOrderBookReader> _orderBookReader;
        private readonly Mock<IOrderBookDataFactory> _orderBookDataFactory;
        private readonly Mock<IOutputWriter> _outputWriter;
        private readonly string _orderBookPath;

        public IOrderBookBuyerData ExpectedBuyerData { get; }
        public IOrderBookSellerData ExpectedSellerData { get; }

        public IMetaExchangeDataSource Sut { get; }

        public MetaExchangeDataSourceDriver()
        {
            _orderBookPath = "somePath";

            IList<OrderBook> orderBooks = new List<OrderBook>
            {
                CreateOrderBook(),
                CreateOrderBook()
            };

            _orderBookReader = new Mock<IOrderBookReader>();
            _orderBookReader.SetupSequence(i => i.ReadOrderBook(_orderBookPath))
                            .ReturnsAsync(orderBooks)
                            .ReturnsAsync(new List<OrderBook>());

            _outputWriter = new Mock<IOutputWriter>();

            ExpectedBuyerData = Mock.Of<IOrderBookBuyerData>();
            ExpectedSellerData = Mock.Of<IOrderBookSellerData>();

            IOrderBookData orderBookData = Mock.Of<IOrderBookData>(i => i.BuyerData == ExpectedBuyerData && i.SellerData == ExpectedSellerData);
            _orderBookDataFactory = new Mock<IOrderBookDataFactory>();
            _orderBookDataFactory.Setup(i => i.Create(orderBooks)).Returns(orderBookData);

            Sut = new MetaExchangeDataSource(_orderBookReader.Object, _orderBookDataFactory.Object, _outputWriter.Object, _orderBookPath);
        }

        private static OrderBook CreateOrderBook()
        {
            return new OrderBook(Guid.NewGuid(), "2022-01-01T00:00:00", 1111, 11, new List<Bid> { new Bid(), new Bid() }, new List<Ask> { new Ask(), new Ask() });
        }

        public void SetInitException()
        {
            _orderBookReader.Setup(i => i.ReadOrderBook(_orderBookPath)).ThrowsAsync(new Exception("test"));
        }

        public void VerifyOutputWriterCalled()
        {
            _outputWriter.Verify(i => i.OutputString(It.IsAny<string>()), Times.Once);
        }
    }
}