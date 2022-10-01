using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.Logic;
using Moq;

namespace MetaExchange.Tests.Logic.SequenceFinderTests
{
    public class FindOptimalSellSequenceTest : FindOptimalSellSequenceDriver
    {
        [Theory]
        [InlineData(15.26, 0.1, 18.2)] // first buyer leftover
        [InlineData(12.34, 0.2, 22.9)] // first buyer exactly the asked amount
        public void FindOptimalSellSequence_FirstBuyerEnough_ReturnsFirstBuyerData(decimal amountBuyer1, decimal amountBuyer2, decimal amountBuyer3)
        {
            SetBuyerAmounts(amountBuyer1, amountBuyer2, amountBuyer3);

            IExchangeResult result = Sut.FindOptimalSellSequence(Amount, BalanceBtc, Buyers);

            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(37020M);
            result.OrderResult.Should().HaveCount(1);
            result.OrderResult[0].Amount.Should().Be(12.34M);
            result.OrderResult[0].Exchange.Should().Be(Buyers[0]);
        }

        [Theory]
        [InlineData(2.8, 4.9, 20.3)] // third buyer leftover
        [InlineData(2.8, 4.9, 4.64)] // all three combined have exactly the asked amount
        public void FindOptimalSellSequence_ThreeBuyersEnough_ReturnsThreeBuyersData(decimal amountBuyer1, decimal amountBuyer2, decimal amountBuyer3)
        {
            SetBuyerAmounts(amountBuyer1, amountBuyer2, amountBuyer3);

            IExchangeResult result = Sut.FindOptimalSellSequence(Amount, BalanceBtc, Buyers);

            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(22840);
            result.OrderResult.Should().HaveCount(3);

            result.OrderResult[0].Amount.Should().Be(2.8M);
            result.OrderResult[0].Exchange.Should().Be(Buyers[0]);
            result.OrderResult[1].Amount.Should().Be(4.9M);
            result.OrderResult[1].Exchange.Should().Be(Buyers[1]);
            result.OrderResult[2].Amount.Should().Be(4.64M);
            result.OrderResult[2].Exchange.Should().Be(Buyers[2]);
        }

        [Fact]
        public void FindOptimalSellSequence_NotEnoughBuyers_ReturnsErrorResult()
        {
            SetBuyerAmounts(0.1M, 0.2M, 0.3M);

            IExchangeResult result = Sut.FindOptimalSellSequence(Amount, BalanceBtc, Buyers);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(Consts.NOT_ENOUGH_BUYERS);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }

        [Fact]
        public void FindOptimalSellSequence_NoBuyers_ReturnsErrorResult()
        {
            SetNoBuyers();

            IExchangeResult result = Sut.FindOptimalSellSequence(Amount, BalanceBtc, Buyers);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(Consts.NOT_ENOUGH_BUYERS);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }

        [Fact]
        public void FindOptimalSellSequence_AmountExceedesBalance_ReturnsErrorResult()
        {
            const decimal amount = 10;
            const decimal balance = 1;

            IExchangeResult result = Sut.FindOptimalSellSequence(amount, balance, Buyers);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(Consts.NOT_ENOUGH_BTC_TO_SELL);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }
    }

    public class FindOptimalSellSequenceDriver
    {
        public decimal Amount { get; }
        public decimal BalanceBtc { get; }

        public IList<Bid> Buyers { get; private set; }

        public ISequenceFinder Sut { get; }

        public FindOptimalSellSequenceDriver()
        {
            Amount = 12.34M;
            BalanceBtc = 30;

            Sut = new SequenceFinder();
        }

        public void SetBuyerAmounts(decimal amountBuyer1, decimal amountBuyer2, decimal amountBuyer3)
        {
            Buyers = new List<Bid>
            {
                Mock.Of<Bid>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountBuyer1 && j.Price == 3000)),
                Mock.Of<Bid>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountBuyer2 && j.Price == 2000)),
                Mock.Of<Bid>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountBuyer3 && j.Price == 1000))
            };
        }

        public void SetNoBuyers()
        {
            Buyers = new List<Bid>();
        }
    }
}