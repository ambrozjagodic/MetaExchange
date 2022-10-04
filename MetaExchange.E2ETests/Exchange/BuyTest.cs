using FluentAssertions;
using MetaExchange.E2ETests.Core;
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
            HttpRequestMessage request = CreateRequest(1.12M);
            ExchangeResult expected = CreateExpectedResult();

            HttpResponseMessage response = await TestClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            ExchangeResult exchangeResult = JsonConvert.DeserializeObject<ExchangeResult>(responseContent);
            exchangeResult.Should().BeEquivalentTo(expected, i => i.WithStrictOrdering().Excluding(j => j.Id));
        }
    }

    public class BuyDriver : ExchangeBaseDriver
    {
        public static HttpRequestMessage CreateRequest(decimal amount)
        {
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("api/exchange", UriKind.Relative),
                Content = JsonContent.Create(new UserOrder { Type = "Buy", Amount = amount, BalanceBTC = 2.931M, BalanceEur = 7892.31M })
            };
        }

        public static ExchangeResult CreateExpectedResult()
        {
            return new ExchangeResult
            {
                Success = true,
                ErrorMsg = "",
                TotalPrice = 3320.00480M,
                OrderResult = new List<OrderResult>
                {
                    new OrderResult
                    {
                        Amount = 0.405M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder(0.405M, 2964.29M)
                        }
                    },
                    new OrderResult
                    {
                        Amount = 0.405M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder(0.405M, 2964.29M)
                        }
                    },
                    new OrderResult
                    {
                        Amount = 0.310M,
                        Exchange = new ExchangeOrder
                        {
                            Order = CreateOrder(0.405M, 2964.29M)
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
                Type = "Sell",
                Kind = "Limit",
                Amount = amount,
                Price = price
            };
        }
    }
}