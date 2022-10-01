using MetaExchange.Core;

namespace MetaExchange.Validation
{
    public interface IWebAPIRequestValidation
    {
        string ValidateUserOrder(IUserOrder userOrder);
    }
}