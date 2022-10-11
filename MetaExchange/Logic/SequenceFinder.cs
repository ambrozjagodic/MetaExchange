using MetaExchange.Core;

namespace MetaExchange.Logic
{
    public class SequenceFinder : ISequenceFinder
    {
        private readonly ISequenceFinderHelper _helper;

        public SequenceFinder(ISequenceFinderHelper helper)
        {
            _helper = helper;
        }

        public IExchangeResult FindOptimalBuySequence(decimal amount, IOrderBookSellerData sellerData)
        {
            IList<IOrderResult> results = new List<IOrderResult>();

            IDictionary<Guid, decimal> exchangeBalances = new Dictionary<Guid, decimal>();

            int index = 0;
            decimal amountLeftToBuy = amount;
            decimal currentPrice = 0;
            while (amountLeftToBuy != 0 && index < sellerData.Sellers.Count)
            {
                Ask currentSeller = sellerData.Sellers[index];

                Guid exchangeId = sellerData.ExchangeSellerMappings[currentSeller.Order.Id];

                if (!exchangeBalances.TryGetValue(exchangeId, out decimal exchangeBalanceEur))
                {
                    exchangeBalanceEur = sellerData.ExchangeBalanceEur[exchangeId];

                    exchangeBalances.Add(exchangeId, exchangeBalanceEur);
                }

                decimal buyAmount = _helper.GetBuyAmount(amountLeftToBuy, currentSeller, exchangeBalanceEur);
                if (buyAmount != 0)
                {
                    IOrderResult newOrder = new OrderResult(buyAmount, exchangeId, currentSeller);
                    results.Add(newOrder);

                    currentPrice += buyAmount * currentSeller.Order.Price;
                    amountLeftToBuy -= buyAmount;

                    exchangeBalances[exchangeId] -= buyAmount * currentSeller.Order.Price;
                }

                index++;
            }

            if (amountLeftToBuy != 0)
            {
                return new ExchangeResult(new List<IOrderResult>(), 0, Consts.NOT_ENOUGH_BTC_TO_BUY);
            }

            return new ExchangeResult(results, currentPrice, string.Empty);
        }

        public IExchangeResult FindOptimalSellSequence(decimal amount, IOrderBookBuyerData buyerData)
        {
            IList<IOrderResult> results = new List<IOrderResult>();

            IDictionary<Guid, decimal> exchangeBalances = new Dictionary<Guid, decimal>();

            int index = 0;
            decimal amountLeftToSell = amount;
            decimal currentPrice = 0;
            while (amountLeftToSell != 0 && index < buyerData.Buyers.Count)
            {
                Bid currentBuyer = buyerData.Buyers[index];

                Guid exchangeId = buyerData.ExchangeBuyerMappings[currentBuyer.Order.Id];

                if (!exchangeBalances.TryGetValue(exchangeId, out decimal exchangeBalanceBtc))
                {
                    exchangeBalanceBtc = buyerData.ExchangeBalanceBtc[exchangeId];

                    exchangeBalances.Add(exchangeId, exchangeBalanceBtc);
                }

                decimal sellAmount = _helper.GetSellAmount(amountLeftToSell, currentBuyer, exchangeBalanceBtc);
                if (sellAmount != 0)
                {
                    IOrderResult newOrder = new OrderResult(sellAmount, exchangeId, currentBuyer);
                    results.Add(newOrder);

                    currentPrice += sellAmount * currentBuyer.Order.Price;
                    amountLeftToSell -= sellAmount;

                    exchangeBalances[exchangeId] -= sellAmount;
                }

                index++;
            }

            if (amountLeftToSell != 0)
            {
                return new ExchangeResult(new List<IOrderResult>(), 0, Consts.NOT_ENOUGH_BTC_TO_SELL);
            }

            return new ExchangeResult(results, currentPrice, string.Empty);
        }
    }
}