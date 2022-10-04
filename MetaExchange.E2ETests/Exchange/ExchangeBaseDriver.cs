using MetaExchange.E2ETests.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace MetaExchange.E2ETests.Exchange
{
    public class ExchangeBaseDriver
    {
        public HttpClient TestClient { get; }

        public ExchangeBaseDriver()
        {
            TestServer testServer = new TestServer(new WebHostBuilder().ConfigureAppConfiguration(ConfigConfiguration).UseStartup<Startup>());
            TestClient = testServer.CreateClient();
        }

        private static void ConfigConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder config)
        {
            string configFilePath = GetConfigFilePath();

            config.SetBasePath(Environment.CurrentDirectory);
            config.AddJsonFile(configFilePath, optional: false, reloadOnChange: true);
        }

        private static string GetConfigFilePath()
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;

            return $"{projectDirectory}/MetaExchange/Config/Settings.E2ETests.json";
        }
    }
}