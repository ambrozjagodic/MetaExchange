using MetaExchange.Core;
using MetaExchange.Extensions;
using MetaExchange.Logic;
using MetaExchange.Validation;
using Microsoft.AspNetCore.Mvc;

namespace MetaExchange.WebAPI
{
    [Route("api/exchange")]
    public class ExchangeController : ControllerBase
    {
        private readonly IMetaExchangeLogic _logic;
        private readonly IWebAPIRequestValidation _validation;
        private readonly IOutputWriter _outputWriter;

        public ExchangeController(IMetaExchangeLogic logic, IWebAPIRequestValidation validation, IOutputWriter outputWriter)
        {
            _logic = logic;
            _validation = validation;
            _outputWriter = outputWriter;
        }

        [HttpPost]
        public async Task<IActionResult> GetOptimalTransactions([FromBody] UserOrder userOrder)
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
                _outputWriter.OutputString($"Unknown error occured. Exception message: {ex.Message}");

                return StatusCode(500);
            }
        }
    }
}