using MetaExchange.Core;
using Newtonsoft.Json;

namespace MetaExchange.Logic
{
    public class FileOrderBookReader : IOrderBookReader
    {
        public async Task<IList<OrderBook>> ReadOrderBook(string path)
        {
            IList<string> lines = await File.ReadAllLinesAsync(path);

            IList<OrderBook> orders = new List<OrderBook>();
            foreach (string line in lines)
            {
                string orderStr = "{" + line.Split(new[] { '{' }, 2).Last();

                OrderBook orderBook = JsonConvert.DeserializeObject<OrderBook>(orderStr);
                if (orderBook != null)
                {
                    orders.Add(orderBook);
                }
                else
                {
                    //log!!!
                }
            }

            return orders;
        }

        public IList<OrderBook> ReadOrderBook(string path, int numberOfOrderBooks)
        {
            IList<string> lines = File.ReadLines(path).Take(numberOfOrderBooks).ToList();

            IList<OrderBook> orders = new List<OrderBook>();
            foreach (string line in lines)
            {
                string orderStr = "{" + line.Split(new[] { '{' }, 2).Last();

                OrderBook orderBook = JsonConvert.DeserializeObject<OrderBook>(orderStr);
                if (orderBook != null)
                {
                    orders.Add(orderBook);
                }
                else
                {
                    //log!!!
                }
            }

            return orders;
        }
    }
}