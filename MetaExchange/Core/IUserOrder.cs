namespace MetaExchange.Core
{
    public interface IUserOrder
    {
        string Type { get; set; }
        decimal Amount { get; set; }
    }
}