using FluentAssertions;
using MetaExchange.Core;

namespace MetaExchange.Tests.Core
{
    public class OrderBookTest
    {
        [Theory]
        [InlineData("2022-01-01T00:00:00")]
        [InlineData("2022-09-09T22:22:22")]
        public void Constructor_Called_PropertiesSet(string acqTime)
        {
            List<Bid> bids = new List<Bid> { new Bid() };
            List<Ask> asks = new List<Ask> { new Ask() };

            OrderBook result = new OrderBook(acqTime, bids, asks);

            result.AcqTime.Should().Be(DateTimeOffset.Parse(acqTime));
            result.Bids.Should().BeEquivalentTo(bids);
            result.Asks.Should().BeEquivalentTo(asks);
        }
    }
}