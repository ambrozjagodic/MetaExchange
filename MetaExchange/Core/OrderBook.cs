namespace MetaExchange.Core
{
    public class OrderBook
    {
        public OrderBook(string acqTime, decimal balanceEur, decimal balanceBtc, List<Bid> bids, List<Ask> asks)
        {
            AcqTime = DateTimeOffset.Parse(acqTime);
            BalanceEur = balanceEur;
            BalanceBtc = balanceBtc;
            Bids = bids;
            Asks = asks;
        }

        public DateTimeOffset AcqTime { get; set; }
        public decimal BalanceEur { get; set; }
        public decimal BalanceBtc { get; set; }
        public List<Bid> Bids { get; set; }
        public List<Ask> Asks { get; set; }
    }
}