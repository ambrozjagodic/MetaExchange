using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Core
{
    [ExcludeFromCodeCoverage]
    public class OrderBookSellerData : IOrderBookSellerData
    {
        public OrderBookSellerData(IList<Ask> sellers, IDictionary<Guid, decimal> exchangeBalanceEur, IDictionary<Guid, Guid> exchangeSellerMappings)
        {
            Sellers = sellers;
            ExchangeBalanceEur = exchangeBalanceEur;
            ExchangeSellerMappings = exchangeSellerMappings;
        }

        public IList<Ask> Sellers { get; }
        public IDictionary<Guid, decimal> ExchangeBalanceEur { get; }
        public IDictionary<Guid, Guid> ExchangeSellerMappings { get; }
    }
}