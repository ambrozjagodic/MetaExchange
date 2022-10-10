using MetaExchange.Core;

namespace MetaExchange.Logic
{
    public interface ISequenceFinder
    {
        IExchangeResult FindOptimalBuySequence(decimal amount, IList<Ask> sellers);

        IExchangeResult FindOptimalSellSequence(decimal amount, IList<Bid> bids);
    }
}