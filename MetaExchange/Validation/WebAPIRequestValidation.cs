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

            if (userOrder.Amount <= 0)
            {
                return "Invalid exchange amount received.";
            }

            return string.Empty;
        }
    }
}