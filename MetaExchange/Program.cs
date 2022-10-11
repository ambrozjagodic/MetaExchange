using MetaExchange;
using MetaExchange.Config;
using MetaExchange.Core;
using MetaExchange.DataSource;
using MetaExchange.Extensions;
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
        IOutputWriter outputWriter = new ConsoleWriter();

        if (args.Any())
        {
            switch (args[0])
            {
                case "-webapp":
                    RunWebApp();
                    break;
                case "-consoleapp":
                    {
                        if (args.Length == 3 &&
                          (args[1].IsBuying() || args[1].IsSelling()) &&
                          decimal.TryParse(args[2], out decimal amount))
                        {
                            RunConsoleApp(args[1], amount);
                        }
                        else
                        {
                            outputWriter.OutputString("Invalid console app arguments given. Use correct order type and amount.");
                        }

                        break;
                    }

                case "-help":
                    outputWriter.OutputString("Use -webapp to run web application.\nUse -consoleapp to run only console application.\n\tIf console application is used, type (buy or sell) and amount of btc need to be given also, in this exact order.");
                    break;
                default:
                    outputWriter.OutputString("Invalid arguments given. Use -help.");
                    break;
            }
        }
        else
        {
            outputWriter.OutputString("No arguments given. Use -help.");
        }
    }

    private static void RunWebApp()
    {
        IWebHost host = new WebHostBuilder()
           .UseKestrel()
           .ConfigureAppConfiguration(ConfigConfiguration)
           .UseStartup<Startup>()
           .Build();

        host.Run();
    }

    private static void RunConsoleApp(string orderType, decimal amount)
    {
        string configFilePath = GetConfigFilePath();

        string configStr = File.ReadAllText(configFilePath);
        ConfigWrapper config = JsonConvert.DeserializeObject<ConfigWrapper>(configStr);

        ISequenceFinder sequenceFinder = new SequenceFinder(new SequenceFinderHelper());
        IOutputWriter consoleWriter = new ConsoleWriter();
        IMetaExchangeDataSource dataSource = new MetaExchangeDataSource(new FileOrderBookReader(), new OrderBookDataFactory(), consoleWriter, config.OrderBookFilePath);
        IMetaExchangeLogic metaExchangeLogic = new MetaExchangeLogic(sequenceFinder, dataSource);

        IExchangeResult result;
        if (orderType.IsBuying())
        {
            result = metaExchangeLogic.BuyOptimal(new UserOrder { Type = "Buy", Amount = amount }).GetAwaiter().GetResult();
        }
        else
        {
            result = metaExchangeLogic.BuyOptimal(new UserOrder { Type = "sell", Amount = amount }).GetAwaiter().GetResult();
        }

        consoleWriter.OutputResultSequence(result);
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