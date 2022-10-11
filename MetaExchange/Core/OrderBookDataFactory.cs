namespace MetaExchange.Core
{
    public class OrderBookDataFactory : IOrderBookDataFactory
    {
        public IOrderBookData Create(IList<OrderBook> orderBooks)
        {
            IDictionary<Guid, decimal> exchangeBalanceEur = new Dictionary<Guid, decimal>();
            IDictionary<Guid, decimal> exchangeBalanceBtc = new Dictionary<Guid, decimal>();

            IDictionary<Guid, Guid> exchangeSellerMappings = new Dictionary<Guid, Guid>();
            IDictionary<Guid, Guid> exchangeBuyerMappings = new Dictionary<Guid, Guid>();

            foreach (OrderBook exchange in orderBooks)
            {
                exchangeBalanceEur.Add(exchange.Id, exchange.BalanceEur);
                exchangeBalanceBtc.Add(exchange.Id, exchange.BalanceBtc);

                foreach (Ask ask in exchange.Asks)
                {
                    exchangeSellerMappings.Add(ask.Order.Id, exchange.Id);
                }

                foreach (Bid bid in exchange.Bids)
                {
                    exchangeBuyerMappings.Add(bid.Order.Id, exchange.Id);
                }
            }

            IList<Ask> sellers = orderBooks.SelectMany(i => i.Asks)
                .OrderBy(i => i.Order.Price)
                .ThenByDescending(i => i.Order.Amount)
                .ToList();

            IList<Bid> buyers = orderBooks.SelectMany(i => i.Bids)
                .OrderByDescending(i => i.Order.Price)
                .ThenByDescending(i => i.Order.Amount)
                .ToList();

            IOrderBookSellerData sellerData = new OrderBookSellerData(sellers, exchangeBalanceEur, exchangeSellerMappings);
            IOrderBookBuyerData buyerData = new OrderBookBuyerData(buyers, exchangeBalanceBtc, exchangeBuyerMappings);

            return new OrderBookData(sellerData, buyerData);
        }
    }
}