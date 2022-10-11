using MetaExchange.Core;

namespace MetaExchange.DataSource
{
    public interface IMetaExchangeDataSource
    {
        Task Init();

        Task<IOrderBookBuyerData> GetBuyersData();

        Task<IOrderBookSellerData> GetSellersData();
    }
}