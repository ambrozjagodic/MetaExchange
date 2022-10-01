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
        public async Task GetOrderedBuyers_Success_BidsInitializedAndReturned()
        {
            IList<Bid> result = await Sut.GetOrderedBuyers();

            result.Should().BeEquivalentTo(ExpectedBids, i => i.WithStrictOrdering());
        }

        [Fact]
        public async Task GetOrderedBuyers_AlreadyInitialized_InitCalledOnceAndBidsReturned()
        {
            await Sut.Init();

            IList<Bid> result = await Sut.GetOrderedBuyers();

            result.Should().BeEquivalentTo(ExpectedBids, i => i.WithStrictOrdering());
        }

        [Fact]
        public async Task GetOrderedSellers_Success_AsksInitializedAndReturned()
        {
            IList<Ask> result = await Sut.GetOrderedSellers();

            result.Should().BeEquivalentTo(ExpectedAsks, i => i.WithStrictOrdering());
        }

        [Fact]
        public async Task GetOrderedSellers_AlreadyInitialized_InitCalledOnceAndBidsReturned()
        {
            await Sut.Init();

            IList<Ask> result = await Sut.GetOrderedSellers();

            result.Should().BeEquivalentTo(ExpectedAsks, i => i.WithStrictOrdering());
        }
    }

    public class MetaExchangeDataSourceDriver
    {
        private readonly Mock<IOrderBookReader> _orderBookReader;
        private readonly string _orderBookPath;

        public IList<Bid> ExpectedBids { get; }
        public IList<Ask> ExpectedAsks { get; }

        public IMetaExchangeDataSource Sut { get; }

        public MetaExchangeDataSourceDriver()
        {
            _orderBookPath = "somePath";

            Bid bid1 = new() { Order = new Order { Price = 10, Amount = 10 } };
            Bid bid2 = new() { Order = new Order { Price = 10, Amount = 1 } };
            Bid bid3 = new() { Order = new Order { Price = 20, Amount = 1 } };
            Bid bid4 = new() { Order = new Order { Price = 10, Amount = 5 } };

            Ask ask1 = new() { Order = new Order { Price = 20, Amount = 2 } };
            Ask ask2 = new() { Order = new Order { Price = 10, Amount = 1 } };
            Ask ask3 = new() { Order = new Order { Price = 30, Amount = 1 } };
            Ask ask4 = new() { Order = new Order { Price = 20, Amount = 10 } };

            IList<OrderBook> orderBooks = new List<OrderBook>
            {
                new OrderBook("2022-01-01T00:00:00", new List<Bid> { bid1, bid2 }, new List<Ask> { ask1, ask2 } ),
                new OrderBook("2022-01-02T00:00:00", new List<Bid> { bid3, bid4 }, new List<Ask> { ask3, ask4 } )
            };

            _orderBookReader = new Mock<IOrderBookReader>();
            _orderBookReader.SetupSequence(i => i.ReadOrderBook(_orderBookPath))
                            .ReturnsAsync(orderBooks)
                            .ReturnsAsync(new List<OrderBook>());

            ExpectedBids = new List<Bid> { bid3, bid1, bid4, bid2 };
            ExpectedAsks = new List<Ask> { ask2, ask4, ask1, ask3 };

            Sut = new MetaExchangeDataSource(_orderBookReader.Object, _orderBookPath);
        }
    }
}