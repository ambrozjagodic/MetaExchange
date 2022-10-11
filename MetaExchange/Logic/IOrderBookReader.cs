using MetaExchange.Core;

namespace MetaExchange.Logic
{
    public interface IOrderBookReader
    {
        Task<IList<OrderBook>> ReadOrderBook(string path);
    }
}