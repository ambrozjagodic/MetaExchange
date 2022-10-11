using MetaExchange.Core;
using MetaExchange.DataSource;

namespace MetaExchange.Logic
{
    public class MetaExchangeLogic : IMetaExchangeLogic
    {
        private readonly ISequenceFinder _sequenceFinder;
        private readonly IMetaExchangeDataSource _dataSource;

        public MetaExchangeLogic(ISequenceFinder sequenceFinder, IMetaExchangeDataSource dataSource)
        {
            _sequenceFinder = sequenceFinder;
            _dataSource = dataSource;
        }

        public async Task<IExchangeResult> BuyOptimal(IUserOrder userOrder)
        {
            IOrderBookSellerData sellerData = await _dataSource.GetSellersData();
            if (sellerData is null)
            {
                return new ExchangeResult(new List<IOrderResult>(), 0, Consts.ORDER_BOOK_ERROR);
            }

            return _sequenceFinder.FindOptimalBuySequence(userOrder.Amount, sellerData);
        }

        public async Task<IExchangeResult> SellOptimal(IUserOrder userOrder)
        {
            IOrderBookBuyerData buyersData = await _dataSource.GetBuyersData();
            if (buyersData is null)
            {
                return new ExchangeResult(new List<IOrderResult>(), 0, Consts.ORDER_BOOK_ERROR);
            }

            return _sequenceFinder.FindOptimalSellSequence(userOrder.Amount, buyersData);
        }
    }
}