using MetaExchange;
using MetaExchange.Core;
using MetaExchange.Logic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 1)
        {
            string filePath = @"D:\Work\Private\MetaExchangeTask\metaExchangeTask_backend_slo\order_books_data";

            IOrderBookReader reader = new FileOrderBookReader();
            IList<OrderBook> orderBooks = reader.ReadOrderBook(filePath, 10);

            //NewLogic newLogic = new NewLogic();
            //newLogic.ProcessOrderBooks(orderBooks);

            Console.WriteLine("Success.");
        }
        else
        {
            IWebHost host = new WebHostBuilder()
            .UseKestrel()
            .UseStartup<Startup>()
            .Build();

            host.Run();
        }
    }
}