namespace MetaExchange.Core
{
    public interface IExchangeResult
    {
        Guid Id { get; }
        bool Success { get; }
        string ErrorMsg { get; }
        decimal TotalPrice { get; }
        IList<IOrderResult> OrderResult { get; }
    }
}