using MetaExchange.Core;
using MetaExchange.Logic;

namespace MetaExchange.DataSource
{
    public class MetaExchangeDataSource : IMetaExchangeDataSource
    {
        private IOrderBookData _orderBookData;

        private readonly IOrderBookReader _orderBookReader;
        private readonly IOrderBookDataFactory _orderBookDataFactory;
        private readonly IOutputWriter _outputWriter;
        private readonly string _orderBookPath;

        private readonly SemaphoreSlim _semaphore;

        public MetaExchangeDataSource(IOrderBookReader orderBookReader, IOrderBookDataFactory orderBookDataFactory, IOutputWriter outputWriter, string orderBookPath)
        {
            _orderBookReader = orderBookReader;
            _orderBookDataFactory = orderBookDataFactory;
            _outputWriter = outputWriter;
            _orderBookPath = orderBookPath;

            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task Init()
        {
            try
            {
                IList<OrderBook> orderBooks = await _orderBookReader.ReadOrderBook(_orderBookPath);

                _orderBookData = _orderBookDataFactory.Create(orderBooks);
            }
            catch (Exception ex)
            {
                _outputWriter.OutputString($"Error occured while retrieving order book data. Exception message: {ex.Message}");
            }
        }

        public async Task<IOrderBookBuyerData> GetBuyersData()
        {
            await _semaphore.WaitAsync();

            if (_orderBookData is null)
            {
                await Init();
            }

            _semaphore.Release();

            return _orderBookData?.BuyerData;
        }

        public async Task<IOrderBookSellerData> GetSellersData()
        {
            await _semaphore.WaitAsync();

            if (_orderBookData is null)
            {
                await Init();
            }

            _semaphore.Release();

            return _orderBookData?.SellerData;
        }
    }
}