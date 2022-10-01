using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.Logic;
using Moq;

namespace MetaExchange.Tests.Logic.SequenceFinderTests
{
    public class FindOptimalBuySequenceTest : FindOptimalBuySequenceDriver
    {
        [Theory]
        [InlineData(15.26, 0.1, 18.2)] // first seller leftover
        [InlineData(12.34, 0.2, 22.9)] // first seller exactly the asked amount
        public void FindOptimalBuySequence_FirstSellerHasEnough_ReturnsFirstSellerData(decimal amountSeller1, decimal amountSeller2, decimal amountSeller3)
        {
            SetSellerAmounts(amountSeller1, amountSeller2, amountSeller3);

            IExchangeResult result = Sut.FindOptimalBuySequence(Amount, BalanceEur, Sellers);

            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(12340);
            result.OrderResult.Should().HaveCount(1);
            result.OrderResult[0].Amount.Should().Be(12.34M);
            result.OrderResult[0].Exchange.Should().Be(Sellers[0]);
        }

        [Theory]
        [InlineData(2.8, 4.9, 20.3)] // third seller leftover
        [InlineData(2.8, 4.9, 4.64)] // all three combined have exactly the asked amount
        public void FindOptimalBuySequence_ThreeSellersHaveEnough_ReturnsThreeSellersData(decimal amountSeller1, decimal amountSeller2, decimal amountSeller3)
        {
            SetSellerAmounts(amountSeller1, amountSeller2, amountSeller3);

            IExchangeResult result = Sut.FindOptimalBuySequence(Amount, BalanceEur, Sellers);

            result.Success.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.TotalPrice.Should().Be(12354.18M);
            result.OrderResult.Should().HaveCount(3);

            result.OrderResult[0].Amount.Should().Be(2.8M);
            result.OrderResult[0].Exchange.Should().Be(Sellers[0]);
            result.OrderResult[1].Amount.Should().Be(4.9M);
            result.OrderResult[1].Exchange.Should().Be(Sellers[1]);
            result.OrderResult[2].Amount.Should().Be(4.64M);
            result.OrderResult[2].Exchange.Should().Be(Sellers[2]);
        }

        [Theory]
        [InlineData(2.8)] // less than buy amount
        [InlineData(40.92)] // more than buy amount
        public void FindOptimalBuySequence_BudgetExceededOnFirstSeller_ReturnsErrorResult(decimal amountSeller1)
        {
            SetBudgetExceededOnFirstSeller(amountSeller1);

            IExchangeResult result = Sut.FindOptimalBuySequence(Amount, BalanceEur, Sellers);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(Consts.BUDGET_EXCEEDED_ERROR_MSG);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }

        [Fact]
        public void FindOptimalBuySequence_BudgetExceededOnThirdSeller_ReturnsErrorResult()
        {
            SetBudgetExceededOnThirdSeller(1, 2, 40);

            IExchangeResult result = Sut.FindOptimalBuySequence(Amount, BalanceEur, Sellers);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(Consts.BUDGET_EXCEEDED_ERROR_MSG);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }

        [Fact]
        public void FindOptimalBuySequence_NotEnoughBTCToBuy_ReturnsErrorResult()
        {
            SetSellerAmounts(0.1M, 0.2M, 0.3M);

            IExchangeResult result = Sut.FindOptimalBuySequence(Amount, BalanceEur, Sellers);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(Consts.NOT_ENOUGH_BTC_TO_BUY);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }

        [Fact]
        public void FindOptimalBuySequence_NoSellers_ReturnsErrorResult()
        {
            SetNoSellers();

            IExchangeResult result = Sut.FindOptimalBuySequence(Amount, BalanceEur, Sellers);

            result.Success.Should().BeFalse();
            result.ErrorMsg.Should().Be(Consts.NOT_ENOUGH_BTC_TO_BUY);
            result.TotalPrice.Should().Be(0M);
            result.OrderResult.Should().BeEmpty();
        }
    }

    public class FindOptimalBuySequenceDriver
    {
        public decimal Amount { get; }
        public decimal BalanceEur { get; }

        public IList<Ask> Sellers { get; private set; }

        public ISequenceFinder Sut { get; }

        public FindOptimalBuySequenceDriver()
        {
            Amount = 12.34M;
            BalanceEur = 40000;

            Sut = new SequenceFinder();
        }

        public void SetSellerAmounts(decimal amountSeller1, decimal amountSeller2, decimal amountSeller3)
        {
            Sellers = new List<Ask>
            {
                Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller1 && j.Price == 1000)),
                Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller2 && j.Price == 1001)),
                Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller3 && j.Price == 1002))
            };
        }

        public void SetBudgetExceededOnFirstSeller(decimal amountSeller1)
        {
            Sellers = new List<Ask>
            {
                Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller1 && j.Price == 99999)),
                Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == 15.321M && j.Price == 1001)),
                Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == 30.123M && j.Price == 1002))
            };
        }

        public void SetBudgetExceededOnThirdSeller(decimal amountSeller1, decimal amountSeller2, decimal amountSeller3)
        {
            Sellers = new List<Ask>
            {
                Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller1 && j.Price == 10000)),
                Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller2 && j.Price == 15000)),
                Mock.Of<Ask>(i => i.Order == Mock.Of<Order>(j => j.Amount == amountSeller3 && j.Price == 30000))
            };
        }

        public void SetNoSellers()
        {
            Sellers = new List<Ask>();
        }
    }
}