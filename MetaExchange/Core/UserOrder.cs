namespace MetaExchange.Core
{
    public class UserOrder : IUserOrder
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceEur { get; set; }
        public decimal BalanceBTC { get; set; }
    }
}