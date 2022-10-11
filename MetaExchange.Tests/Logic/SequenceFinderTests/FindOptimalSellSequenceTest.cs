using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.Logic;
using MetaExchange.Tests.Core;
using Moq;

namespace MetaExchange.Tests.Logic.SequenceFinderTests
{
    public class FindOptimalSellSequenceTest : FindOptimalSellSequenceDriver
    {
        [Fact]
        public void FindOptimalSellSequence_FirstBuyerHasEnough_ReturnsExchangeResult()
        {
            SetFirstBuyerHasEnough();

            IExchangeResult result = Sut.FindOptimalSellSequence(Amount, BuyerData);

            result.Id.Should().NotBeEmpty();
            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(12340);
            result.OrderResult.Should().BeEquivalentTo(ExpectedOrderResult);
        }

        [Fact]
        public void FindOptimalSellSequence_TwoBuyersSameExchange_ReturnsExchangeResult()
        {
            SetTwoBuyersSameExchange();

            IExchangeResult result = Sut.FindOptimalSellSequence(Amount, BuyerData);

            result.Id.Should().NotBeEmpty();
            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(14680);
            result.OrderResult.Should().BeEquivalentTo(ExpectedOrderResult);
        }

        [Fact]
        public void FindOptimalSellSequence_TwoBuyersSameExchangeBalanceReached_ReturnsEmtpyResult()
        {
            SetTwoBuyersSameExchangeBalanceReached();

            IExchangeResult result = Sut.FindOptimalSellSequence(Amount, BuyerData);

            result.Id.Should().NotBeEmpty();
            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(Consts.NOT_ENOUGH_BTC_TO_SELL);
            result.TotalPrice.Should().Be(0);
            result.OrderResult.Should().BeEmpty();
        }

        [Fact]
        public void FindOptimalSellSequence_NoBuyers_ReturnsEmpty()
        {
            SetNoBuyers();

            IExchangeResult result = Sut.FindOptimalSellSequence(Amount, BuyerData);

            result.Id.Should().NotBeEmpty();
            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(Consts.NOT_ENOUGH_BTC_TO_SELL);
            result.TotalPrice.Should().Be(0);
            result.OrderResult.Should().BeEmpty();
        }
    }

    public class FindOptimalSellSequenceDriver
    {
        private readonly Mock<ISequenceFinderHelper> _helper;

        private readonly IList<Bid> _buyers;

        public decimal Amount { get; }
        public IOrderBookBuyerData BuyerData { get; private set; }
        public IList<IOrderResult> ExpectedOrderResult { get; private set; }

        public ISequenceFinder Sut { get; }

        public FindOptimalSellSequenceDriver()
        {
            Amount = 12.34M;

            _buyers = new List<Bid>
            {
                new Bid { Order = new Order { Id = TestConsts.GUID_1, Price = 1000 } },
                new Bid { Order = new Order { Id = TestConsts.GUID_2, Price = 2000 } }
            };

            _helper = new Mock<ISequenceFinderHelper>();

            Sut = new SequenceFinder(_helper.Object);
        }

        public void SetFirstBuyerHasEnough()
        {
            IDictionary<Guid, decimal> exchangeBalanceBtc = new Dictionary<Guid, decimal>
            {
                [TestConsts.GUID_4] = 20,
                [TestConsts.GUID_5] = 30
            };

            IDictionary<Guid, Guid> exchangeBuyerMappings = new Dictionary<Guid, Guid>
            {
                [TestConsts.GUID_1] = TestConsts.GUID_4,
                [TestConsts.GUID_2] = TestConsts.GUID_5
            };

            BuyerData = Mock.Of<IOrderBookBuyerData>(i => i.Buyers == _buyers && i.ExchangeBalanceBtc == exchangeBalanceBtc && i.ExchangeBuyerMappings == exchangeBuyerMappings);

            ExpectedOrderResult = new List<IOrderResult>
            {
                Mock.Of<IOrderResult>(i => i.ExchangeId == TestConsts.GUID_4 && i.Amount == 12.34M && i.Exchange == _buyers[0])
            };

            _helper.Setup(i => i.GetSellAmount(Amount, _buyers[0], exchangeBalanceBtc[TestConsts.GUID_4])).Returns(12.34M);
        }

        public void SetTwoBuyersSameExchange()
        {
            IDictionary<Guid, decimal> exchangeBalanceBtc = new Dictionary<Guid, decimal>
            {
                [TestConsts.GUID_3] = 20
            };

            IDictionary<Guid, Guid> exchangeBuyerMappings = new Dictionary<Guid, Guid>
            {
                [TestConsts.GUID_1] = TestConsts.GUID_3,
                [TestConsts.GUID_2] = TestConsts.GUID_3,
            };

            BuyerData = Mock.Of<IOrderBookBuyerData>(i => i.Buyers == _buyers && i.ExchangeBalanceBtc == exchangeBalanceBtc && i.ExchangeBuyerMappings == exchangeBuyerMappings);

            ExpectedOrderResult = new List<IOrderResult>
            {
                Mock.Of<IOrderResult>(i => i.ExchangeId == TestConsts.GUID_3 && i.Amount == 10M && i.Exchange == _buyers[0]),
                Mock.Of<IOrderResult>(i => i.ExchangeId == TestConsts.GUID_3 && i.Amount == 2.34M && i.Exchange == _buyers[1])
            };

            _helper.Setup(i => i.GetSellAmount(12.34M, _buyers[0], 20)).Returns(10M);
            _helper.Setup(i => i.GetSellAmount(2.34M, _buyers[1], 10)).Returns(2.34M);
        }

        public void SetTwoBuyersSameExchangeBalanceReached()
        {
            IDictionary<Guid, decimal> exchangeBalanceBtc = new Dictionary<Guid, decimal>
            {
                [TestConsts.GUID_3] = 10
            };

            IDictionary<Guid, Guid> exchangeBuyerMappings = new Dictionary<Guid, Guid>
            {
                [TestConsts.GUID_1] = TestConsts.GUID_3,
                [TestConsts.GUID_2] = TestConsts.GUID_3,
            };

            BuyerData = Mock.Of<IOrderBookBuyerData>(i => i.Buyers == _buyers && i.ExchangeBalanceBtc == exchangeBalanceBtc && i.ExchangeBuyerMappings == exchangeBuyerMappings);

            ExpectedOrderResult = new List<IOrderResult>
            {
                Mock.Of<IOrderResult>(i => i.ExchangeId == TestConsts.GUID_3 && i.Amount == 10M && i.Exchange == _buyers[0]),
                Mock.Of<IOrderResult>(i => i.ExchangeId == TestConsts.GUID_3 && i.Amount == 2.34M && i.Exchange == _buyers[1])
            };

            _helper.Setup(i => i.GetSellAmount(12.34M, _buyers[0], 10)).Returns(10M);
            _helper.Setup(i => i.GetSellAmount(2.34M, _buyers[1], 0)).Returns(0);
        }

        public void SetNoBuyers()
        {
            BuyerData = Mock.Of<IOrderBookBuyerData>(i => i.Buyers == new List<Bid>());
        }
    }
}