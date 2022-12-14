using MetaExchange.Core;
using Newtonsoft.Json;

namespace MetaExchange.Logic
{
    public class FileOrderBookReader : IOrderBookReader
    {
        public async Task<IList<OrderBook>> ReadOrderBook(string path)
        {
            IList<string> lines = await File.ReadAllLinesAsync(path);

            IList<OrderBook> orders = ParseLines(lines);

            return orders;
        }

        private static IList<OrderBook> ParseLines(IList<string> lines)
        {
            IList<OrderBook> orderBooks = new List<OrderBook>();
            foreach (string line in lines)
            {
                string orderStr = "{" + line.Split(new[] { '{' }, 2).Last();

                OrderBook orderBook = JsonConvert.DeserializeObject<OrderBook>(orderStr);
                if (orderBook != null)
                {
                    orderBooks.Add(orderBook);
                }
            }

            return orderBooks;
        }
    }
}