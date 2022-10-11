using MetaExchange.Core;

namespace MetaExchange.Logic
{
    public interface ISequenceFinderHelper
    {
        decimal GetBuyAmount(decimal amountLeftToBuy, Ask currentSeller, decimal exchangeBalanceEur);

        decimal GetSellAmount(decimal amountLeftToSell, Bid currentBuyer, decimal exchangeBalanceBtc);
    }
}