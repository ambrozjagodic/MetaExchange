using MetaExchange.Core;
using MetaExchange.Logic;

namespace MetaExchange.DataSource
{
    public class MetaExchangeDataSource : IMetaExchangeDataSource
    {
        private IList<OrderBook> _orderBooks = new List<OrderBook>();
        private IList<Ask> _asks = new List<Ask>();
        private IList<Bid> _bids = new List<Bid>();

        private readonly IOrderBookReader _orderBookReader;
        private readonly IOutputWriter _outputWriter;
        private readonly string _orderBookPath;

        private readonly SemaphoreSlim _semaphore;

        public MetaExchangeDataSource(IOrderBookReader orderBookReader, IOutputWriter outputWriter, string orderBookPath)
        {
            _orderBookReader = orderBookReader;
            _outputWriter = outputWriter;
            _orderBookPath = orderBookPath;

            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task Init()
        {
            try
            {
                _orderBooks = await _orderBookReader.ReadOrderBook(_orderBookPath);

                _asks = _orderBooks.SelectMany(i => i.Asks)
                    .OrderBy(i => i.Order.Price)
                    .ThenByDescending(i => i.Order.Amount)
                    .ToList();

                _bids = _orderBooks.SelectMany(i => i.Bids)
                    .OrderByDescending(i => i.Order.Price)
                    .ThenByDescending(i => i.Order.Amount)
                    .ToList();
            }
            catch (Exception ex)
            {
                _outputWriter.OutputString($"Error occured while retrieving order book data. Exception message: {ex.Message}");
            }
        }

        public IList<OrderBook> GetLastNumberOfOrderBooks(int numberOfBooks)
        {
            if (_orderBooks.Any())
            {
                return _orderBooks.TakeLast(numberOfBooks).ToList();
            }

            return _orderBookReader.ReadNumberOfOrderBooks(_orderBookPath, numberOfBooks);
        }

        public async Task<IList<Bid>> GetOrderedBuyers()
        {
            await _semaphore.WaitAsync();

            if (!_bids.Any())
            {
                await Init();
            }

            _semaphore.Release();

            return _bids;
        }

        public async Task<IList<Ask>> GetOrderedSellers()
        {
            await _semaphore.WaitAsync();

            if (!_asks.Any())
            {
                await Init();
            }

            _semaphore.Release();

            return _asks;
        }
    }
}