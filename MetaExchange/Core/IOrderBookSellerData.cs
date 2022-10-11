namespace MetaExchange.Core
{
    public interface IOrderBookSellerData
    {
        IList<Ask> Sellers { get; }
        IDictionary<Guid, decimal> ExchangeBalanceEur { get; }
        IDictionary<Guid, Guid> ExchangeSellerMappings { get; }
    }
}