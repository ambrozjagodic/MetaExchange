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
            HttpRequestMessage request = CreateRequest(1.92M);
            ExchangeResult expected = CreateExpectedResult();

            HttpResponseMessage response = await TestClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            ExchangeResult exchangeResult = JsonConvert.DeserializeObject<ExchangeResult>(responseContent);
            exchangeResult.Should().BeEquivalentTo(expected, i => i.Excluding(j => j.Id));
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
    }
}