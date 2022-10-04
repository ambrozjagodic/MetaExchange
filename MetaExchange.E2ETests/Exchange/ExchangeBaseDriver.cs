using System;
using MetaExchange.E2ETests.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace MetaExchange.E2ETests.Exchange
{
    public class ExchangeBaseDriver
    {
        public HttpClient TestClient { get; }

        public ExchangeBaseDriver()
        {
            string value = Environment.GetEnvironmentVariable("run-env");
            Environment.SetEnvironmentVariable("run-env", "e2e-tests");

            TestServer testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            TestClient = testServer.CreateClient();
        }

        public static Order CreateOrder(string type, decimal amount, decimal price)
        {
            return new Order
            {
                Id = null,
                Time = "0001-01-01T00:00:00",
                Type = type,
                Kind = "Limit",
                Amount = amount,
                Price = price
            };
        }
    }
}

