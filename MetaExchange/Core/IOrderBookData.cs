namespace MetaExchange.Core
{
    public interface IOrderBookData
    {
        IOrderBookSellerData SellerData { get; }
        IOrderBookBuyerData BuyerData { get; }
    }
}