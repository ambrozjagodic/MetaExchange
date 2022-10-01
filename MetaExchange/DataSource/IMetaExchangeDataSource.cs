using MetaExchange.Core;

namespace MetaExchange.DataSource
{
    public interface IMetaExchangeDataSource
    {
        Task Init();

        Task<IList<Bid>> GetOrderedBuyers();

        Task<IList<Ask>> GetOrderedSellers();
    }
}