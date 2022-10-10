using MetaExchange.Core;
using MetaExchange.DataSource;

namespace MetaExchange.Logic
{
    public class MetaExchangeLogic : IMetaExchangeLogic
    {
        private readonly ISequenceFinder _sequenceFinder;
        private readonly IOutputWriter _outputWriter;
        private readonly IMetaExchangeDataSource _dataSource;

        public MetaExchangeLogic(ISequenceFinder sequenceFinder, IOutputWriter outputWriter, IMetaExchangeDataSource dataSource)
        {
            _sequenceFinder = sequenceFinder;
            _outputWriter = outputWriter;
            _dataSource = dataSource;
        }

        public async Task<IExchangeResult> BuyOptimal(IUserOrder userOrder)
        {
            IList<Ask> sellers = await _dataSource.GetOrderedSellers();

            IExchangeResult exchangeResult = _sequenceFinder.FindOptimalBuySequence(userOrder.Amount, sellers);

            return exchangeResult;
        }

        public async Task<IExchangeResult> SellOptimal(IUserOrder userOrder)
        {
            IList<Bid> buyers = await _dataSource.GetOrderedBuyers();

            IExchangeResult exchangeResult = _sequenceFinder.FindOptimalSellSequence(userOrder.Amount, buyers);

            return exchangeResult;
        }

        public void FindOptimalSequencePerExchange(int numberOfOrderBooks)
        {
            IList<OrderBook> orderBooks = _dataSource.GetLastNumberOfOrderBooks(numberOfOrderBooks);

            Random random = new();

            foreach (OrderBook orderBook in orderBooks)
            {
                double balanceEur = (random.NextDouble() * 200000);
                double balanceBtc = (random.NextDouble() * 30);
                double amount = random.NextDouble() * 20;

                _outputWriter.OutputInitialValues(balanceEur, balanceBtc, amount);

                IExchangeResult buyResult = _sequenceFinder.FindOptimalBuySequence((decimal)amount, orderBook.Asks);
                IExchangeResult sellResult = _sequenceFinder.FindOptimalSellSequence((decimal)amount, orderBook.Bids);

                _outputWriter.OutputResultSequence(buyResult, sellResult);
            }
        }
    }
}