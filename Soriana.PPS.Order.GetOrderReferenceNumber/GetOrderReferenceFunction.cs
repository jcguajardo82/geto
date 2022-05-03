using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;

using Soriana.PPS.Common.Constants;
using Soriana.PPS.Common.DTO.Common;
using Soriana.PPS.Common.DTO.OrderData;
using Soriana.PPS.Order.GetOrderReferenceNumber.Services;
using Soriana.PPS.Order.GetOrderReferenceNumber.Constants;


namespace Soriana.PPS.Order.GetOrderReferenceNumber
{
    public class GetOrderReferenceFunction
    {
        #region Private Fields
        private readonly ILogger<GetOrderReferenceFunction> _Logger;
        private readonly IOrderReferenceService _OrderReferenceService;
        #endregion

        #region Constructor
        public GetOrderReferenceFunction(ILogger<GetOrderReferenceFunction> logger,
                                         IOrderReferenceService orderReferenceService)
        {
            _Logger = logger;
            _OrderReferenceService = orderReferenceService;
        }
        #endregion

        #region Public Methods
        [FunctionName(OrderReferenceConstants.GET_ORDER_REFERENCE_FUNCTION_NAME)]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request)
        {
            try
            {
                _Logger.LogInformation(string.Format(FunctionAppConstants.FUNCTION_EXECUTING_MESSAGE, OrderReferenceConstants.GET_ORDER_REFERENCE_FUNCTION_NAME));
                if (!request.Body.CanSeek)
                    throw new Exception(JsonConvert.SerializeObject(new BusinessResponse() { StatusCode = (int)HttpStatusCode.BadRequest, Description = HttpStatusCode.BadRequest.ToString(), DescriptionDetail = OrderReferenceConstants.GET_ORDER_REFERENCE_NO_CONTENT_REQUEST, ContentRequest = null }));
                request.Body.Position = 0;
                string jsonPaymentOrderProcessRequest = await new StreamReader(request.Body).ReadToEndAsync();

                OrderReferenceRequest ueNoRequest = JsonConvert.DeserializeObject<OrderReferenceRequest>(jsonPaymentOrderProcessRequest);

                var Response = await _OrderReferenceService.GetOrderReference(ueNoRequest.ShipmentNumber);

                if (Response.OrderReferenceNumber == "")
                    return new OkObjectResult("Orden de compra no disponible");
                else
                    return new OkObjectResult(Response);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, OrderReferenceConstants.GET_ORDER_REFERENCE_FUNCTION_NAME);
                return new BadRequestObjectResult(new BusinessResponse()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Description = string.Concat(HttpStatusCode.InternalServerError.ToString(), CharactersConstants.ESPACE_CHAR, CharactersConstants.HYPHEN_CHAR, CharactersConstants.ESPACE_CHAR, OrderReferenceConstants.GET_ORDER_REFERENCE_FUNCTION_NAME),
                    DescriptionDetail = ex
                });
            }
        }
        #endregion
    }


}
