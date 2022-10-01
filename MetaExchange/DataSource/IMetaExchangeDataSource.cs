using MetaExchange.Core;

namespace MetaExchange.DataSource
{
    public interface IMetaExchangeDataSource
    {
        Task Init();

        IList<OrderBook> GetLastNumberOfOrderBooks(int numberOfBooks);

        Task<IList<Bid>> GetOrderedBuyers();

        Task<IList<Ask>> GetOrderedSellers();
    }
}