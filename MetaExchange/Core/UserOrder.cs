using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Core
{
    [ExcludeFromCodeCoverage]
    public class UserOrder : IUserOrder
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
    }
}