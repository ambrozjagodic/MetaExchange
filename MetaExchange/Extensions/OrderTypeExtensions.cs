namespace MetaExchange.Extensions
{
    public static class OrderTypeExtensions
    {
        public static bool IsBuying(this string orderType)
        {
            return string.Equals(orderType, "Buy", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsSelling(this string orderType)
        {
            return string.Equals(orderType, "Sell", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}