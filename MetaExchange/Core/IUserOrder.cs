namespace MetaExchange.Core
{
    public interface IUserOrder
    {
        string Type { get; set; }
        decimal Amount { get; set; }
        decimal BalanceEur { get; set; }
        decimal BalanceBTC { get; set; }
    }
}