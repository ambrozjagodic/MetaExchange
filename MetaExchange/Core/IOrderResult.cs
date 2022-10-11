namespace MetaExchange.Core
{
    public interface IOrderResult
    {
        decimal Amount { get; set; }
        Guid ExchangeId { get; set; }
        IExchange Exchange { get; set; }
    }
}