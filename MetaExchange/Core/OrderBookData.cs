using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Core
{
    [ExcludeFromCodeCoverage]
    public class OrderBookData : IOrderBookData
    {
        public OrderBookData(IOrderBookSellerData sellerData, IOrderBookBuyerData buyerData)
        {
            SellerData = sellerData;
            BuyerData = buyerData;
        }

        public IOrderBookSellerData SellerData { get; }
        public IOrderBookBuyerData BuyerData { get; }
    }
}