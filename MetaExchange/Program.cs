using MetaExchange;
using MetaExchange.Config;
using MetaExchange.Core;
using MetaExchange.DataSource;
using MetaExchange.Logic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        if (args.Any())
        {
            if (args.Length == 3 && args[0] == "-consoleapp" && (args[1] == "buy" || args[1] == "sell") && decimal.TryParse(args[2], out decimal amount))
            {
                string configFilePath = GetConfigFilePath();

                string configStr = File.ReadAllText(configFilePath);
                ConfigWrapper config = JsonConvert.DeserializeObject<ConfigWrapper>(configStr);

                ISequenceFinder sequenceFinder = new SequenceFinder(new SequenceFinderHelper());
                IOutputWriter consoleWriter = new ConsoleWriter();
                IMetaExchangeDataSource dataSource = new MetaExchangeDataSource(new FileOrderBookReader(), new OrderBookDataFactory(), consoleWriter, config.OrderBookFilePath);

                IMetaExchangeLogic metaExchangeLogic = new MetaExchangeLogic(sequenceFinder, dataSource);
                if (args[1] == "buy")
                {
                    IExchangeResult result = metaExchangeLogic.BuyOptimal(new UserOrder { Type = "Buy", Amount = amount }).GetAwaiter().GetResult();
                }
                else if (args[1] == "sell")
                {
                    IExchangeResult result = metaExchangeLogic.BuyOptimal(new UserOrder { Type = "sell", Amount = amount }).GetAwaiter().GetResult();
                }
            }
            else if (args[0] == "-webapp")
            {
                IWebHost host = new WebHostBuilder()
                   .UseKestrel()
                   .ConfigureAppConfiguration(ConfigConfiguration)
                   .UseStartup<Startup>()
                   .Build();

                host.Run();
            }
            else if (args[0] == "-help")
            {
                Console.WriteLine("Use -webapp to run web application.\nUse -consoleapp to run only console application.\n\tIf console application is used, type (buy or sell) and amount of btc need to be given also, in this order.");
            }
            else
            {
                Console.WriteLine("Invalid arguments given. Use -help.");
            }
        }
        else
        {
            Console.WriteLine("No arguments given. Use -help.");
        }
    }

    private static void ConfigConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder config)
    {
        string configFilePath = GetConfigFilePath();

        config.SetBasePath(Environment.CurrentDirectory);
        config.AddJsonFile(configFilePath, optional: false, reloadOnChange: true);
    }

    private static string GetConfigFilePath()
    {
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;

        return $"{projectDirectory}/Config/Settings.json";
    }
}