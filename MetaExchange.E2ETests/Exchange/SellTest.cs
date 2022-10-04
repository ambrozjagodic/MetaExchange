using FluentAssertions;
using MetaExchange.E2ETests.Core;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace MetaExchange.E2ETests.Exchange
{
    public class SellTest : SellDriver
    {
        [Fact]
        public async Task Sell_Valid_ReturnsSellSequence()
        {
            HttpRequestMessage request = CreateRequest(1.13M);
            ExchangeResult expected = CreateExpectedResult();

            HttpResponseMessage response = await TestClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            ExchangeResult exchangeResult = JsonConvert.DeserializeObject<ExchangeResult>(responseContent);
            exchangeResult.Should().BeEquivalentTo(expected, i => i.WithStrictOrdering().Excluding(j => j.Id));
        }
    }

    public class SellDriver : ExchangeBaseDriver
    {
        public static HttpRequestMessage CreateRequest(decimal amount)
        {
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("api/exchange", UriKind.Relative),
                Content = JsonContent.Create(new UserOrder { Type = "Sell", Amount = amount, BalanceBTC = 2.931M, BalanceEur = 7892.31M })
            };
        }

        public ExchangeResult CreateExpectedResult()
        {
            return new ExchangeResult
            {
                Success = true,
                ErrorMsg = "",
                TotalPrice = 3345.5346117578M,
                OrderResult = new List<OrderResult>
                {
                    new OrderResult
                    {
                        Amount = 0.01M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder(0.01M, 2960.67M)
                        }
                    },
                    new OrderResult
                    {
                        Amount = 1.11117578M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder(1.11117578M, 2960.65M)
                        }
                    },
                    new OrderResult
                    {
                        Amount = 0.00882422M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder(0.01M, 2960.64M)
                        }
                    }
                }
            };
        }

        public static Order CreateOrder(decimal amount, decimal price)
        {
            return new Order
            {
                Id = null,
                Time = "0001-01-01T00:00:00",
                Type = "Buy",
                Kind = "Limit",
                Amount = amount,
                Price = price
            };
        }
    }
}