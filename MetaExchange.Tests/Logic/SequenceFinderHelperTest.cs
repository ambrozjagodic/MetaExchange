using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.Logic;

namespace MetaExchange.Tests.Logic
{
    public class SequenceFinderHelperTest : SequenceFinderHelperDriver
    {
        [Fact]
        public void GetBuyAmount_BalanceOkAndSellerHasEnough_ReturnsUsersAmount()
        {
            decimal userAmount = 2;
            decimal sellerAmount = 10;
            decimal sellerPrice = 1000;
            decimal exchangeBalance = 10000;

            decimal result = Sut.GetBuyAmount(userAmount, new Ask { Order = new Order { Amount = sellerAmount, Price = sellerPrice } }, exchangeBalance);

            result.Should().Be(userAmount);
        }

        [Fact]
        public void GetBuyAmount_BalanceOkAndSellerDoesNotHaveEnough_ReturnsSellerAmount()
        {
            decimal userAmount = 2;
            decimal sellerAmount = 1.5M;
            decimal sellerPrice = 1000;
            decimal exchangeBalance = 10000;

            decimal result = Sut.GetBuyAmount(userAmount, new Ask { Order = new Order { Amount = sellerAmount, Price = sellerPrice } }, exchangeBalance);

            result.Should().Be(sellerAmount);
        }

        [Fact]
        public void GetBuyAmount_BalanceNotOkAndSellerHasEnough_ReturnsFullBalanceAmount()
        {
            decimal userAmount = 2;
            decimal sellerAmount = 10M;
            decimal sellerPrice = 1000;
            decimal exchangeBalance = 1500;

            decimal result = Sut.GetBuyAmount(userAmount, new Ask { Order = new Order { Amount = sellerAmount, Price = sellerPrice } }, exchangeBalance);

            result.Should().Be(1.5M);
        }

        [Fact]
        public void GetBuyAmount_NoBalance_ReturnsZero()
        {
            decimal noBalance = 0;

            decimal result = Sut.GetBuyAmount(1, new Ask { Order = new Order { Amount = 2, Price = 1000 } }, noBalance);

            result.Should().Be(0);
        }

        [Fact]
        public void GetSellAmount_BalanceOkAndBuyerHasEnough_ReturnsUsersAmount()
        {
            decimal userAmount = 2;
            decimal buyerAmount = 10;
            decimal buyerPrice = 1000;
            decimal exchangeBalance = 5;

            decimal result = Sut.GetSellAmount(userAmount, new Bid { Order = new Order { Amount = buyerAmount, Price = buyerPrice } }, exchangeBalance);

            result.Should().Be(userAmount);
        }

        [Fact]
        public void GetSellAmount_BalanceOkAndBuyerDoesNotHaveEnough_ReturnsBuyerAmount()
        {
            decimal userAmount = 2;
            decimal buyerAmount = 1.5M;
            decimal buyerPrice = 1000;
            decimal exchangeBalance = 5;

            decimal result = Sut.GetSellAmount(userAmount, new Bid { Order = new Order { Amount = buyerAmount, Price = buyerPrice } }, exchangeBalance);

            result.Should().Be(buyerAmount);
        }

        [Fact]
        public void GetSellAmount_BalanceNotOkAndSellerHasEnough_ReturnsFullBalanceAmount()
        {
            decimal userAmount = 2;
            decimal buyerAmount = 10M;
            decimal buyerPrice = 1000;
            decimal exchangeBalance = 1.5M;

            decimal result = Sut.GetSellAmount(userAmount, new Bid { Order = new Order { Amount = buyerAmount, Price = buyerPrice } }, exchangeBalance);

            result.Should().Be(1.5M);
        }

        [Fact]
        public void GetSellAmount_NoBalance_ReturnsZero()
        {
            decimal noBalance = 0;

            decimal result = Sut.GetSellAmount(1, new Bid { Order = new Order { Amount = 2, Price = 1000 } }, noBalance);

            result.Should().Be(0);
        }
    }

    public class SequenceFinderHelperDriver
    {
        public ISequenceFinderHelper Sut { get; }

        public SequenceFinderHelperDriver()
        {
            Sut = new SequenceFinderHelper();
        }
    }
}