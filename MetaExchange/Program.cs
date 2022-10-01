using MetaExchange;
using MetaExchange.Core;
using MetaExchange.Logic;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class Program
{
    private const string BUY = "Buy";
    private const string SELL = "Sell";

    private static async Task Main(string[] args)
    {
        //if (args.Length == 1)
        {
            string filePath = @"D:\Work\Private\MetaExchangeTask\metaExchangeTask_backend_slo\order_books_data";

            IOrderBookReader reader = new FileOrderBookReader();
            IList<OrderBook> orderBooks = reader.ReadOrderBook(filePath, 10);

            NewLogic newLogic = new NewLogic();
            newLogic.ProcessOrderBooks(orderBooks);

            Console.WriteLine("Success.");
        }
       /* else
        {
            IWebHost host = new WebHostBuilder()
            .UseKestrel()
            .UseStartup<Startup>()
            .Build();

            host.Run();
        }*/
        /*args = GetMockArgs();

        Console.WriteLine($"Received order of type {args[0]} for {args[1]} BTC. Balance EUR = {args[2]}; Balance BTC = {args[3]}");

        IUserOrder userOrder = new UserOrder(args);

        string filePath = @"D:\Work\Private\MetaExchangeTask\metaExchangeTask_backend_slo\order_books_data";

        //string filePath = @"D:\Work\Private\MetaExchangeTask\metaExchangeTask_backend_slo\order_books_data_small";

        IOrderBookReader reader = new FileOrderBookReader();

        IMetaExchangeDataSource dataSource = new MetaExchangeDataSource(reader);
        await dataSource.Init(filePath);

        IMetaExchangeLogic logic = new MetaExchangeLogic(dataSource);

        if (userOrder.Type.IsBuying())
        {
            logic.BuyOptimal(userOrder);
        }
        else if (userOrder.Type.IsSelling())
        {
            logic.SellOptimal(userOrder);
        }
        else
        {
            Console.WriteLine("Fuck off.");
        }*/
    }
}