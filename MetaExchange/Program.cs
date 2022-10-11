﻿using MetaExchange;
using MetaExchange.DataSource;
using MetaExchange.Logic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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

                ISequenceFinder sequenceFinder = new SequenceFinder(new SequenceFinderHelper());
                IOutputWriter consoleWriter = new ConsoleWriter();
                IMetaExchangeDataSource dataSource = null;// new MetaExchangeDataSource(new FileOrderBookReader(), consoleWriter, filePath);

                IMetaExchangeLogic metaExchangeLogic = null;// new MetaExchangeLogic(sequenceFinder, consoleWriter, dataSource);
                //metaExchangeLogic.FindOptimalSequencePerExchange(numberOfOrderBooks);
            }
        }
        else
        {
            IWebHost host = new WebHostBuilder()
                .UseKestrel()
                .ConfigureAppConfiguration(ConfigConfiguration)
                .UseStartup<Startup>()
                .Build();

            host.Run();
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