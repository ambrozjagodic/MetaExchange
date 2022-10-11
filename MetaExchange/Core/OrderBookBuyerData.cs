using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Core
{
    [ExcludeFromCodeCoverage]
    public class OrderBookBuyerData : IOrderBookBuyerData
    {
        public OrderBookBuyerData(IList<Bid> buyers, IDictionary<Guid, decimal> exchangeBalanceBtc, IDictionary<Guid, Guid> exchangeBuyerMappings)
        {
            Buyers = buyers;
            ExchangeBalanceBtc = exchangeBalanceBtc;
            ExchangeBuyerMappings = exchangeBuyerMappings;
        }

        public IList<Bid> Buyers { get; }
        public IDictionary<Guid, decimal> ExchangeBalanceBtc { get; }
        public IDictionary<Guid, Guid> ExchangeBuyerMappings { get; }
    }
}