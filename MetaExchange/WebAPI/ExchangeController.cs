using MetaExchange.Core;
using MetaExchange.Extensions;
using MetaExchange.Logic;
using MetaExchange.Validation;
using Microsoft.AspNetCore.Mvc;

namespace MetaExchange.WebAPI
{
    public class ExchangeController : ControllerBase
    {
        private readonly IMetaExchangeLogic _logic;
        private readonly IWebAPIRequestValidation _validation;

        public ExchangeController(IMetaExchangeLogic logic, IWebAPIRequestValidation validation)
        {
            _logic = logic;
            _validation = validation;
        }

        [HttpPost]
        [Route("exchange")]
        public async Task<IActionResult> Post([FromBody] UserOrder userOrder)
        {
            try
            {
                string errorMsg = _validation.ValidateUserOrder(userOrder);
                if (string.IsNullOrEmpty(errorMsg))
                {
                    IExchangeResult result;
                    if (userOrder.Type.IsBuying())
                    {
                        result = await _logic.BuyOptimal(userOrder);
                    }
                    else
                    {
                        result = await _logic.SellOptimal(userOrder);
                    }

                    return Created($"/{result.Id}", result);
                }
                
                return BadRequest(errorMsg);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500);
            }
        }
    }
}