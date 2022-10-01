using MetaExchange;
using MetaExchange.DataSource;
using MetaExchange.Logic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 1)
        {
            if (int.TryParse(args[0], out int numberOfOrderBooks))
            {
                string filePath = @"D:\Work\Private\MetaExchangeTask\metaExchangeTask_backend_slo\order_books_data";

                ISequenceFinder sequenceFinder = new SequenceFinder();
                IOutputWriter consoleWriter = new ConsoleWriter();
                IMetaExchangeDataSource dataSource = new MetaExchangeDataSource(new FileOrderBookReader(), consoleWriter, filePath);

                IMetaExchangeLogic metaExchangeLogic = new MetaExchangeLogic(sequenceFinder, consoleWriter, dataSource);
                metaExchangeLogic.FindOptimalSequencePerExchange(numberOfOrderBooks);
            }
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