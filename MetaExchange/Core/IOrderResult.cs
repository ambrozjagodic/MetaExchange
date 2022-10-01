namespace MetaExchange.Core
{
    public interface IOrderResult
    {
        decimal Amount { get; set; }
        IExchange Exchange { get; set; }
    }
}