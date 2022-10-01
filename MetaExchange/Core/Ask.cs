using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Core
{
    [ExcludeFromCodeCoverage]
    public class Ask : IExchange
    {
        public Order Order { get; set; }
    }
}