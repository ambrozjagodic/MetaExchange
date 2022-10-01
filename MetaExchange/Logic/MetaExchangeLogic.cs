using MetaExchange.Core;
using MetaExchange.DataSource;

namespace MetaExchange.Logic
{
    public class MetaExchangeLogic : IMetaExchangeLogic
    {
        private const string BUDGET_EXCEEDED_ERROR_MSG = "Budget exceeded.";
        private const string NOT_ENOUGH_BTC_TO_BUY = "Not enough BTC to buy.";
        private const string NOT_ENOUGH_BUYERS = "Not enough BTC buyers.";

        private readonly IMetaExchangeDataSource _dataSource;

        public MetaExchangeLogic(IMetaExchangeDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IExchangeResult> BuyOptimal(IUserOrder userOrder)
        {
            IList<Ask> sellers = await _dataSource.GetOrderedSellers();

            IList<IOrderResult> results = new List<IOrderResult>();

            bool budgetExceeded = false;
            int index = 0;
            decimal amountLeftToBuy = userOrder.Amount;
            decimal currentPrice = 0;
            while (amountLeftToBuy != 0 && !budgetExceeded && index < sellers.Count)
            {
                Ask currentSeller = sellers[index];

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

                budgetExceeded = userOrder.BalanceEur < currentPrice;

                index++;
            }

            if (budgetExceeded)
            {
                return new ExchangeResult(new List<IOrderResult>(), 0, BUDGET_EXCEEDED_ERROR_MSG);
            }

            if (amountLeftToBuy != 0)
            {
                return new ExchangeResult(new List<IOrderResult>(), 0, NOT_ENOUGH_BTC_TO_BUY);
            }

            return new ExchangeResult(results, currentPrice, string.Empty);
        }

        public async Task<IExchangeResult> SellOptimal(IUserOrder userOrder)
        {
            IList<Bid> buyers = await _dataSource.GetOrderedBuyers();

            IList<IOrderResult> results = new List<IOrderResult>();

            int index = 0;
            decimal amountLeftToSell = userOrder.Amount;
            decimal currentPrice = 0;
            while (amountLeftToSell != 0 && index < buyers.Count)
            {
                Bid currentBuyer = buyers[index];

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
                return new ExchangeResult(new List<IOrderResult>(), 0, NOT_ENOUGH_BUYERS);
            }

            return new ExchangeResult(results, currentPrice, string.Empty);
        }
    }
}