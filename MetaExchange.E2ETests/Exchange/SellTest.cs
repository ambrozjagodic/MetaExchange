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
            HttpRequestMessage request = CreateRequest(0.03M);
            ExchangeResult expected = CreateExpectedResult();

            HttpResponseMessage response = await TestClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            ExchangeResult exchangeResult = JsonConvert.DeserializeObject<ExchangeResult>(responseContent);
            exchangeResult.Should().BeEquivalentTo(expected, i => i.Excluding(j => j.Id));
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
    }
}