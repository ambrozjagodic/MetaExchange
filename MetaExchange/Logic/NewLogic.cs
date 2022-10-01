using MetaExchange.Core;
using Newtonsoft.Json;

namespace MetaExchange.Logic
{
    public class NewLogic
    {
        private const string BUDGET_EXCEEDED_ERROR_MSG = "Budget exceeded.";
        private const string NOT_ENOUGH_BTC_TO_BUY = "Not enough BTC to buy.";
        private const string NOT_ENOUGH_BTC_TO_SELL = "Not enough BTC to sell.";
        private const string NOT_ENOUGH_BUYERS = "Not enough BTC buyers.";

        private readonly Random _random = new Random();

        public void ProcessOrderBooks(IList<OrderBook> orderBooks)
        {
            foreach (OrderBook orderBook in orderBooks)
            {
                double balanceEur = (_random.NextDouble() * 200000);
                double balanceBtc = (_random.NextDouble() * 30);
                double amount = _random.NextDouble() * 20;

                OutputInputToConsole(balanceEur, balanceBtc, amount);

                IExchangeResult buyResult = Buy((decimal)amount, (decimal)balanceEur, orderBook.Asks);
                IExchangeResult sellResult = Sell((decimal)amount, (decimal)balanceBtc, orderBook.Bids);

                OutputResultToConsole(buyResult, sellResult);
            }
        }

        private void OutputInputToConsole(double balanceEur, double balanceBtc, double amount)
        {
            Console.WriteLine("###############################################################################################################");
            Console.WriteLine("###############################################################################################################");
            Console.WriteLine("###############################################################################################################");
            Console.WriteLine("###############################################################################################################");
            Console.WriteLine("###############################################################################################################");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Processing input: \n\tBalanceEur={balanceEur};\n\tBalanceBTC={balanceBtc};\n\tAmount={amount};");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        private void OutputResultToConsole(IExchangeResult buyResult, IExchangeResult sellResult)
        {
            Console.WriteLine("Buy results:");
            Console.WriteLine();
            Console.WriteLine();
            string buyResultStr = JsonConvert.SerializeObject(buyResult);
            Console.WriteLine(buyResultStr);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Sell results:");
            Console.WriteLine();
            Console.WriteLine();
            string sellResultStr = JsonConvert.SerializeObject(sellResult);
            Console.WriteLine(sellResultStr);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        private IExchangeResult Buy(decimal amount, decimal balanceEur, List<Ask> sellers)
        {
            IList<IOrderResult> results = new List<IOrderResult>();

            bool budgetExceeded = false;
            int index = 0;
            decimal amountLeftToBuy = amount;
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

                budgetExceeded = balanceEur < currentPrice;

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

        private IExchangeResult Sell(decimal amount, decimal balanceBtc, List<Bid> bids)
        {
            if (balanceBtc < amount)
            {
                return new ExchangeResult(new List<IOrderResult>(), 0, NOT_ENOUGH_BTC_TO_SELL);
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
                return new ExchangeResult(new List<IOrderResult>(), 0, NOT_ENOUGH_BUYERS);
            }

            return new ExchangeResult(results, currentPrice, string.Empty);
        }
    }
}