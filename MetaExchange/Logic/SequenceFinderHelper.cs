using MetaExchange.Core;

namespace MetaExchange.Logic
{
    public class SequenceFinderHelper : ISequenceFinderHelper
    {
        public decimal GetBuyAmount(decimal amountLeftToBuy, Ask currentSeller, decimal exchangeBalanceEur)
        {
            decimal amount = currentSeller.Order.Amount;
            if (amountLeftToBuy < currentSeller.Order.Amount)
            {
                amount = amountLeftToBuy;
            }

            if (exchangeBalanceEur < (amount * currentSeller.Order.Price))
            {
                amount = exchangeBalanceEur / currentSeller.Order.Price;
            }

            return amount;
        }

        public decimal GetSellAmount(decimal amountLeftToSell, Bid currentBuyer, decimal exchangeBalanceBtc)
        {
            IList<decimal> amounts = new List<decimal> { amountLeftToSell, currentBuyer.Order.Amount, exchangeBalanceBtc };

            return amounts.Min();
        }
    }
}