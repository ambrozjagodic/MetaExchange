using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.Logic;
using MetaExchange.Tests.Core;
using Moq;

namespace MetaExchange.Tests.Logic.SequenceFinderTests
{
    public class FindOptimalBuySequenceTest : FindOptimalBuySequenceDriver
    {
        [Fact]
        public void FindOptimalBuySequence_FirstSellerHasEnough_ReturnsExchangeResult()
        {
            SetFirstSellerHasEnough();

            IExchangeResult result = Sut.FindOptimalBuySequence(Amount, SellerData);

            result.Id.Should().NotBeEmpty();
            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(12340);
            result.OrderResult.Should().BeEquivalentTo(ExpectedOrderResult);
        }

        [Fact]
        public void FindOptimalBuySequence_TwoSellersSameExchange_ReturnsExchangeResult()
        {
            SetTwoSellersSameExchange();

            IExchangeResult result = Sut.FindOptimalBuySequence(Amount, SellerData);

            result.Id.Should().NotBeEmpty();
            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(14680);
            result.OrderResult.Should().BeEquivalentTo(ExpectedOrderResult);
        }

        [Fact]
        public void FindOptimalBuySequence_TwoSellersSameExchangeBalanceReached_ReturnsEmtpyResult()
        {
            SetTwoSellersSameExchangeBalanceReached();

            IExchangeResult result = Sut.FindOptimalBuySequence(Amount, SellerData);

            result.Id.Should().NotBeEmpty();
            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(Consts.NOT_ENOUGH_BTC_TO_BUY);
            result.TotalPrice.Should().Be(0);
            result.OrderResult.Should().BeEmpty();
        }

        [Fact]
        public void FindOptimalBuySequence_NoSellers_ReturnsEmpty()
        {
            SetNoSellers();

            IExchangeResult result = Sut.FindOptimalBuySequence(Amount, SellerData);

            result.Id.Should().NotBeEmpty();
            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(Consts.NOT_ENOUGH_BTC_TO_BUY);
            result.TotalPrice.Should().Be(0);
            result.OrderResult.Should().BeEmpty();
        }
    }

    public class FindOptimalBuySequenceDriver
    {
        private readonly Mock<ISequenceFinderHelper> _helper;

        private readonly IList<Ask> _sellers;

        public decimal Amount { get; }
        public IOrderBookSellerData SellerData { get; private set; }
        public IList<IOrderResult> ExpectedOrderResult { get; private set; }

        public ISequenceFinder Sut { get; }

        public FindOptimalBuySequenceDriver()
        {
            Amount = 12.34M;

            _sellers = new List<Ask>
            {
                new Ask { Order = new Order { Id = TestConsts.GUID_1, Price = 1000 } },
                new Ask { Order = new Order { Id = TestConsts.GUID_2, Price = 2000 } }
            };

            _helper = new Mock<ISequenceFinderHelper>();

            Sut = new SequenceFinder(_helper.Object);
        }

        public void SetFirstSellerHasEnough()
        {
            IDictionary<Guid, decimal> exchangeBalanceEur = new Dictionary<Guid, decimal>
            {
                [TestConsts.GUID_4] = 10000,
                [TestConsts.GUID_5] = 20000
            };

            IDictionary<Guid, Guid> exchangeSellerMappings = new Dictionary<Guid, Guid>
            {
                [TestConsts.GUID_1] = TestConsts.GUID_4,
                [TestConsts.GUID_2] = TestConsts.GUID_5
            };

            SellerData = Mock.Of<IOrderBookSellerData>(i => i.Sellers == _sellers && i.ExchangeBalanceEur == exchangeBalanceEur && i.ExchangeSellerMappings == exchangeSellerMappings);

            ExpectedOrderResult = new List<IOrderResult>
            {
                Mock.Of<IOrderResult>(i => i.ExchangeId == TestConsts.GUID_4 && i.Amount == 12.34M && i.Exchange == _sellers[0])
            };

            _helper.Setup(i => i.GetBuyAmount(Amount, _sellers[0], exchangeBalanceEur[TestConsts.GUID_4])).Returns(12.34M);
        }

        public void SetTwoSellersSameExchange()
        {
            IDictionary<Guid, decimal> exchangeBalanceEur = new Dictionary<Guid, decimal>
            {
                [TestConsts.GUID_3] = 20000
            };

            IDictionary<Guid, Guid> exchangeSellerMappings = new Dictionary<Guid, Guid>
            {
                [TestConsts.GUID_1] = TestConsts.GUID_3,
                [TestConsts.GUID_2] = TestConsts.GUID_3,
            };

            SellerData = Mock.Of<IOrderBookSellerData>(i => i.Sellers == _sellers && i.ExchangeBalanceEur == exchangeBalanceEur && i.ExchangeSellerMappings == exchangeSellerMappings);

            ExpectedOrderResult = new List<IOrderResult>
            {
                Mock.Of<IOrderResult>(i => i.ExchangeId == TestConsts.GUID_3 && i.Amount == 10M && i.Exchange == _sellers[0]),
                Mock.Of<IOrderResult>(i => i.ExchangeId == TestConsts.GUID_3 && i.Amount == 2.34M && i.Exchange == _sellers[1])
            };

            _helper.Setup(i => i.GetBuyAmount(12.34M, _sellers[0], 20000)).Returns(10M);
            _helper.Setup(i => i.GetBuyAmount(2.34M, _sellers[1], 10000)).Returns(2.34M);
        }

        public void SetTwoSellersSameExchangeBalanceReached()
        {
            IDictionary<Guid, decimal> exchangeBalanceEur = new Dictionary<Guid, decimal>
            {
                [TestConsts.GUID_3] = 10000
            };

            IDictionary<Guid, Guid> exchangeSellerMappings = new Dictionary<Guid, Guid>
            {
                [TestConsts.GUID_1] = TestConsts.GUID_3,
                [TestConsts.GUID_2] = TestConsts.GUID_3,
            };

            SellerData = Mock.Of<IOrderBookSellerData>(i => i.Sellers == _sellers && i.ExchangeBalanceEur == exchangeBalanceEur && i.ExchangeSellerMappings == exchangeSellerMappings);

            ExpectedOrderResult = new List<IOrderResult>
            {
                Mock.Of<IOrderResult>(i => i.ExchangeId == TestConsts.GUID_3 && i.Amount == 10M && i.Exchange == _sellers[0]),
                Mock.Of<IOrderResult>(i => i.ExchangeId == TestConsts.GUID_3 && i.Amount == 2.34M && i.Exchange == _sellers[1])
            };

            _helper.Setup(i => i.GetBuyAmount(12.34M, _sellers[0], 10000)).Returns(10M);
            _helper.Setup(i => i.GetBuyAmount(2.34M, _sellers[1], 0)).Returns(0);
        }

        public void SetNoSellers()
        {
            SellerData = Mock.Of<IOrderBookSellerData>(i => i.Sellers == new List<Ask>());
        }
    }
}