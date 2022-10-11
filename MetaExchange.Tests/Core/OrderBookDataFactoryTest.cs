using FluentAssertions;
using MetaExchange.Core;

namespace MetaExchange.Tests.Core
{
    public class OrderBookDataFactoryTest : OrderBookDataFactoryDriver
    {
        [Fact]
        public void Create_Called_OrderBookDataReturned()
        {
            IOrderBookData result = Sut.Create(OrderBooks);

            result.SellerData.Sellers.Should().BeEquivalentTo(ExpectedSellers, i => i.WithStrictOrdering());
            result.SellerData.ExchangeBalanceEur.Should().BeEquivalentTo(ExpectedExchangeBalanceEur);
            result.SellerData.ExchangeSellerMappings.Should().BeEquivalentTo(ExpectedExchangeSellerMappings);

            result.BuyerData.Buyers.Should().BeEquivalentTo(ExpectedBuyers, i => i.WithStrictOrdering());
            result.BuyerData.ExchangeBalanceBtc.Should().BeEquivalentTo(ExpectedExchangeBalanceBtc);
            result.BuyerData.ExchangeBuyerMappings.Should().BeEquivalentTo(ExpectedExchangeBuyerMappings);
        }
    }

    public class OrderBookDataFactoryDriver
    {
        public IList<OrderBook> OrderBooks { get; }

        public IList<Ask> ExpectedSellers { get; }
        public IList<Bid> ExpectedBuyers { get; }
        public IDictionary<Guid, decimal> ExpectedExchangeBalanceEur { get; }
        public IDictionary<Guid, decimal> ExpectedExchangeBalanceBtc { get; }
        public IDictionary<Guid, Guid> ExpectedExchangeSellerMappings { get; }
        public IDictionary<Guid, Guid> ExpectedExchangeBuyerMappings { get; }

        public IOrderBookDataFactory Sut { get; }

        public OrderBookDataFactoryDriver()
        {
            List<Bid> bids1 = new List<Bid>
            {
                new Bid { Order = new Order{ Id = TestConsts.GUID_3, Amount = 1, Price = 222 } },
                new Bid { Order = new Order{ Id = TestConsts.GUID_4, Amount = 1, Price = 111 } }
            };

            List<Bid> bids2 = new List<Bid>
            {
                new Bid { Order = new Order{ Id = TestConsts.GUID_5, Amount = 1, Price = 444 } },
                new Bid { Order = new Order{ Id = TestConsts.GUID_6, Amount = 2, Price = 222 } }
            };

            List<Ask> asks1 = new List<Ask>
            {
                new Ask { Order = new Order{ Id = TestConsts.GUID_7, Amount = 1, Price = 222 } },
                new Ask { Order = new Order{ Id = TestConsts.GUID_8, Amount = 1, Price = 111 } }
            };

            List<Ask> asks2 = new List<Ask>
            {
                new Ask { Order = new Order{ Id = TestConsts.GUID_9, Amount = 1, Price = 444 } },
                new Ask { Order = new Order{ Id = TestConsts.GUID_10, Amount = 2, Price = 222 } }
            };

            OrderBooks = new List<OrderBook>
            {
                new OrderBook(TestConsts.GUID_1, "2019-01-29T11:00:00.2518854Z", 1111, 11, bids1, asks1),
                new OrderBook(TestConsts.GUID_2, "2019-01-29T11:00:00.2518854Z", 2222, 22, bids2, asks2)
            };

            ExpectedSellers = new List<Ask>
            {
                asks1[1],
                asks2[1],
                asks1[0],
                asks2[0]
            };

            ExpectedBuyers = new List<Bid>
            {
                bids2[0],
                bids2[1],
                bids1[0],
                bids1[1]
            };

            ExpectedExchangeBalanceEur = new Dictionary<Guid, decimal>
            {
                [TestConsts.GUID_1] = 1111,
                [TestConsts.GUID_2] = 2222
            };

            ExpectedExchangeBalanceBtc = new Dictionary<Guid, decimal>
            {
                [TestConsts.GUID_1] = 11,
                [TestConsts.GUID_2] = 22
            };

            ExpectedExchangeBuyerMappings = new Dictionary<Guid, Guid>
            {
                [TestConsts.GUID_3] = TestConsts.GUID_1,
                [TestConsts.GUID_4] = TestConsts.GUID_1,
                [TestConsts.GUID_5] = TestConsts.GUID_2,
                [TestConsts.GUID_6] = TestConsts.GUID_2
            };

            ExpectedExchangeSellerMappings = new Dictionary<Guid, Guid>
            {
                [TestConsts.GUID_7] = TestConsts.GUID_1,
                [TestConsts.GUID_8] = TestConsts.GUID_1,
                [TestConsts.GUID_9] = TestConsts.GUID_2,
                [TestConsts.GUID_10] = TestConsts.GUID_2
            };

            Sut = new OrderBookDataFactory();
        }
    }
}