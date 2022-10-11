namespace MetaExchange.Core
{
    public interface IOrderBookDataFactory
    {
        IOrderBookData Create(IList<OrderBook> orderBooks);
    }
}