using MetaExchange.Core;

namespace MetaExchange.Logic
{
    public interface ISequenceFinder
    {
        IExchangeResult FindOptimalBuySequence(decimal amount, IOrderBookSellerData sellerData);

        IExchangeResult FindOptimalSellSequence(decimal amount, IOrderBookBuyerData buyerData);
    }
}