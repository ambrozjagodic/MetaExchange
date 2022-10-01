namespace MetaExchange.Core
{
    [Serializable]
    public class OrderBook
    {
        public OrderBook(string acqTime, List<Bid> bids, List<Ask> asks)
        {
            AcqTime = DateTimeOffset.Parse(acqTime);
            Bids = bids;
            Asks = asks;
        }

        public DateTimeOffset AcqTime { get; set; }
        public List<Bid> Bids { get; set; }
        public List<Ask> Asks { get; set; }
    }
}