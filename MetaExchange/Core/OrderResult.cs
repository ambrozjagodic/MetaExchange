namespace MetaExchange.Core
{
    public class OrderResult : IOrderResult
    {
        public OrderResult(decimal amount, IExchange exchange)
        {
            Amount = amount;
            Exchange = exchange;
        }

        public decimal Amount { get; set; }
        public IExchange Exchange { get; set; }
    }
}