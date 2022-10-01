using MetaExchange.Core;
using MetaExchange.Extensions;

namespace MetaExchange.Validation
{
    public class WebAPIRequestValidation : IWebAPIRequestValidation
    {
        public string ValidateUserOrder(IUserOrder userOrder)
        {
            if (userOrder == null)
            {
                return "Invalid object received (null).";
            }

            if (string.IsNullOrEmpty(userOrder.Type) || (!userOrder.Type.IsBuying() && !userOrder.Type.IsSelling()))
            {
                return "Invalid order type received.";
            }

            if (userOrder.Type.IsBuying() && userOrder.BalanceEur == 0)
            {
                return "Insufficient funds to purchase.";
            }

            if (userOrder.Type.IsSelling() && userOrder.BalanceBTC < userOrder.Amount)
            {
                return "Insufficient funds to sell.";
            }

            if (userOrder.Amount <= 0)
            {
                return "Invalid exchange amount received.";
            }

            return string.Empty;
        }
    }
}