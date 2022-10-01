using MetaExchange.Core;

namespace MetaExchange.Logic
{
    public interface IMetaExchangeLogic
    {
        Task<IExchangeResult> BuyOptimal(IUserOrder userOrder);

        Task<IExchangeResult> SellOptimal(IUserOrder userOrder);

        void FindOptimalSequencePerExchange(IList<OrderBook> orderBooks);
    }
}