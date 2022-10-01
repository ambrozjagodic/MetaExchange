using MetaExchange.Core;

namespace MetaExchange.Logic
{
    public class SequenceFinder : ISequenceFinder
    {
        public IExchangeResult FindOptimalBuySequence(decimal amount, decimal balanceEur, IList<Ask> asks)
        {
            IList<IOrderResult> results = new List<IOrderResult>();

            bool budgetExceeded = false;
            int index = 0;
            decimal amountLeftToBuy = amount;
            decimal currentPrice = 0;
            while (amountLeftToBuy != 0 && !budgetExceeded && index < asks.Count)
            {
                Ask currentSeller = asks[index];

                if (amountLeftToBuy <= currentSeller.Order.Amount)
                {
                    IOrderResult newOrder = new OrderResult(amountLeftToBuy, currentSeller);
                    results.Add(newOrder);

                    currentPrice += amountLeftToBuy * currentSeller.Order.Price;

                    amountLeftToBuy = 0;
                }
                else
                {
                    IOrderResult newOrder = new OrderResult(currentSeller.Order.Amount, currentSeller);
                    results.Add(newOrder);

                    currentPrice += currentSeller.Order.Amount * currentSeller.Order.Price;

                    amountLeftToBuy -= currentSeller.Order.Amount;
                }

                budgetExceeded = balanceEur < currentPrice;

                index++;
            }

            if (budgetExceeded)
            {
                return new ExchangeResult(new List<IOrderResult>(), 0, Consts.BUDGET_EXCEEDED_ERROR_MSG);
            }

            if (amountLeftToBuy != 0)
            {
                return new ExchangeResult(new List<IOrderResult>(), 0, Consts.NOT_ENOUGH_BTC_TO_BUY);
            }

            return new ExchangeResult(results, currentPrice, string.Empty);
        }

        public IExchangeResult FindOptimalSellSequence(decimal amount, decimal balanceBtc, IList<Bid> bids)
        {
            if (balanceBtc < amount)
            {
                return new ExchangeResult(new List<IOrderResult>(), 0, Consts.NOT_ENOUGH_BTC_TO_SELL);
            }

            IList<IOrderResult> results = new List<IOrderResult>();

            int index = 0;
            decimal amountLeftToSell = amount;
            decimal currentPrice = 0;
            while (amountLeftToSell != 0 && index < bids.Count)
            {
                Bid currentBuyer = bids[index];

                if (amountLeftToSell <= currentBuyer.Order.Amount)
                {
                    IOrderResult newOrder = new OrderResult(amountLeftToSell, currentBuyer);
                    results.Add(newOrder);

                    currentPrice += amountLeftToSell * currentBuyer.Order.Price;

                    amountLeftToSell = 0;
                }
                else
                {
                    IOrderResult newOrder = new OrderResult(currentBuyer.Order.Amount, currentBuyer);
                    results.Add(newOrder);

                    currentPrice += currentBuyer.Order.Amount * currentBuyer.Order.Price;

                    amountLeftToSell -= currentBuyer.Order.Amount;
                }

                index++;
            }

            if (amountLeftToSell != 0)
            {
                return new ExchangeResult(new List<IOrderResult>(), 0, Consts.NOT_ENOUGH_BUYERS);
            }

            return new ExchangeResult(results, currentPrice, string.Empty);
        }
    }
}