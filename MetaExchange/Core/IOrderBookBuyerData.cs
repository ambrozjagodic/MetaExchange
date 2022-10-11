namespace MetaExchange.Core
{
    public interface IOrderBookBuyerData
    {
        IList<Bid> Buyers { get; }
        IDictionary<Guid, decimal> ExchangeBalanceBtc { get; }
        IDictionary<Guid, Guid> ExchangeBuyerMappings { get; }
    }
}