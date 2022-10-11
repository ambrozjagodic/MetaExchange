using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Core
{
    [ExcludeFromCodeCoverage]
    public class ExchangeResult : IExchangeResult
    {
        public ExchangeResult(IList<IOrderResult> orderResult, decimal totalPrice, string errorMsg)
        {
            Id = Guid.NewGuid();
            OrderResult = orderResult;
            TotalPrice = totalPrice;
            ErrorMsg = errorMsg;
        }

        public Guid Id { get; }
        public bool Success => OrderResult.Any();
        public string ErrorMsg { get; }
        public decimal TotalPrice { get; }
        public IList<IOrderResult> OrderResult { get; }
    }
}