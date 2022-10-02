namespace MetaExchange.E2ETests.Core
{
    public class ExchangeResult
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string ErrorMsg { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderResult> OrderResult { get; set; }
    }
}