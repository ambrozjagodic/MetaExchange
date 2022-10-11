using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Core
{
    [ExcludeFromCodeCoverage]
    public class OrderResult : IOrderResult
    {
        public OrderResult(decimal amount, Guid exchangeId, IExchange exchange)
        {
            Amount = amount;
            ExchangeId = exchangeId;
            Exchange = exchange;
        }

        public decimal Amount { get; set; }
        public Guid ExchangeId { get; set; }
        public IExchange Exchange { get; set; }
    }
}