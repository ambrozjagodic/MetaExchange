using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Core
{
    [ExcludeFromCodeCoverage]
    public class Bid : IExchange
    {
        public Order Order { get; set; }
    }
}