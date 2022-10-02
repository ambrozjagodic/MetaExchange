using FluentAssertions;
using MetaExchange.E2ETests.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace MetaExchange.E2ETests.Exchange
{
    public class BuyTest : BuyDriver
    {
        [Fact]
        public async Task Buy_Valid_ReturnsBuySequence()
        {
            HttpRequestMessage request = CreateRequest("Buy", 1.92M);
            ExchangeResult expected = CreateExpectedBuyResult();

            HttpResponseMessage response = await TestClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            ExchangeResult exchangeResult = JsonConvert.DeserializeObject<ExchangeResult>(responseContent);
            exchangeResult.Should().BeEquivalentTo(expected, i => i.Excluding(j => j.Id));
        }

        [Fact]
        public async Task Sell_Valid_ReturnsSellSequence()
        {
            HttpRequestMessage request = CreateRequest("Sell", 0.03M);
            ExchangeResult expected = CreateExpectedSellResult();

            HttpResponseMessage response = await TestClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            ExchangeResult exchangeResult = JsonConvert.DeserializeObject<ExchangeResult>(responseContent);
            exchangeResult.Should().BeEquivalentTo(expected, i => i.Excluding(j => j.Id));
        }
    }

    public class BuyDriver
    {
        public HttpClient TestClient { get; }

        public BuyDriver()
        {
            TestServer testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            TestClient = testServer.CreateClient();
        }

        public static HttpRequestMessage CreateRequest(string type, decimal amount)
        {
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("api/exchange", UriKind.Relative),
                Content = JsonContent.Create(new UserOrder { Type = type, Amount = amount, BalanceBTC = 2.931M, BalanceEur = 7892.31M })
            };
        }

        public static ExchangeResult CreateExpectedBuyResult()
        {
            return new ExchangeResult
            {
                Success = true,
                ErrorMsg = "",
                TotalPrice = 5675.8129666M,
                OrderResult = new List<OrderResult>
                {
                    new OrderResult
                    {
                        Amount = 1.18438M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder("Sell", 1.18438M, 2955.03M)
                        }
                    },
                    new OrderResult
                    {
                        Amount = 0.406M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder("Sell", 0.406M, 2957.96M)
                        }
                    },
                    new OrderResult
                    {
                        Amount = 0.32962M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder("Sell", 0.406M, 2957.96M)
                        }
                    }
                }
            };
        }

        public ExchangeResult CreateExpectedSellResult()
        {
            return new ExchangeResult
            {
                Success = true,
                ErrorMsg = "",
                TotalPrice = 89.0085M,
                OrderResult = new List<OrderResult>
                {
                    new OrderResult
                    {
                        Amount = 0.01M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder("Buy", 0.01M, 2966.95M)
                        }
                    },
                    new OrderResult
                    {
                        Amount = 0.01M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder("Buy", 0.01M, 2966.95M)
                        }
                    },
                    new OrderResult
                    {
                        Amount = 0.01M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder("Buy", 0.01M, 2966.95M)
                        }
                    }
                }
            };
        }

        private static Order CreateOrder(string type, decimal amount, decimal price)
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