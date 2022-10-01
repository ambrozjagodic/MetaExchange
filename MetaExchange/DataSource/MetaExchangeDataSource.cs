using MetaExchange.Core;
using MetaExchange.Logic;

namespace MetaExchange.DataSource
{
    public class MetaExchangeDataSource : IMetaExchangeDataSource
    {
        private IList<Ask> _asks = new List<Ask>();
        private IList<Bid> _bids = new List<Bid>();

        private readonly IOrderBookReader _orderBookReader;
        private readonly IOutputWriter _outputWriter;
        private readonly string _orderBookPath;

        public MetaExchangeDataSource(IOrderBookReader orderBookReader, IOutputWriter outputWriter, string orderBookPath)
        {
            _orderBookReader = orderBookReader;
            _outputWriter = outputWriter;
            _orderBookPath = orderBookPath;
        }

        public async Task Init()
        {
            try
            {
                IList<OrderBook> data = await _orderBookReader.ReadOrderBook(_orderBookPath);

                _asks = data.SelectMany(i => i.Asks)
                    .OrderBy(i => i.Order.Price)
                    .ThenByDescending(i => i.Order.Amount)
                    .ToList();

                _bids = data.SelectMany(i => i.Bids)
                    .OrderByDescending(i => i.Order.Price)
                    .ThenByDescending(i => i.Order.Amount)
                    .ToList();
            }
            catch (Exception ex)
            {
                _outputWriter.OutputString($"Error occured while retrieving order book data. Exception message: {ex.Message}");
            }
        }

        public async Task<IList<Bid>> GetOrderedBuyers()
        {
            if (!_bids.Any())
            {
                await Init();
            }

            return _bids;
        }

        public async Task<IList<Ask>> GetOrderedSellers()
        {
            if (!_asks.Any())
            {
                await Init();
            }

            return _asks;
        }
    }
}