using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Core
{
    [ExcludeFromCodeCoverage]
    public class OrderBook
    {
        public OrderBook(Guid id, string acqTime, decimal balanceEur, decimal balanceBtc, List<Bid> bids, List<Ask> asks)
        {
            Id = id;
            AcqTime = DateTimeOffset.Parse(acqTime);
            BalanceEur = balanceEur;
            BalanceBtc = balanceBtc;
            Bids = bids;
            Asks = asks;
        }

        public Guid Id { get; set; }
        public DateTimeOffset AcqTime { get; set; }
        public decimal BalanceEur { get; set; }
        public decimal BalanceBtc { get; set; }
        public List<Bid> Bids { get; set; }
        public List<Ask> Asks { get; set; }
    }
}