using MetaExchange.Core;

namespace MetaExchange.Logic
{
    public interface ISequenceFinder
    {
        IExchangeResult FindOptimalBuySequence(decimal amount, decimal balanceEur, IList<Ask> sellers);

        IExchangeResult FindOptimalSellSequence(decimal amount, decimal balanceBtc, IList<Bid> bids);
    }
}