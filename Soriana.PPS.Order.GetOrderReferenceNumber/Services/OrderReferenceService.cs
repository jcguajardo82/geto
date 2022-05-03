using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Soriana.PPS.Common.DTO.OrderData;
using Soriana.PPS.DataAccess.PendingPayments;

namespace Soriana.PPS.Order.GetOrderReferenceNumber.Services
{
    public class OrderReferenceService : IOrderReferenceService
    {
        #region Private Fields
        public ILogger<OrderReferenceService> _Logger;
        public IPendingPaymentsContext _PendingPaymentsContext;
        #endregion

        #region Constructors
        public OrderReferenceService(ILogger<OrderReferenceService> logger,
                                     IPendingPaymentsContext pendingPaymentsContext)
        {
            _Logger = logger;
            _PendingPaymentsContext = pendingPaymentsContext;
        }
        #endregion

        #region Public Methods
        public async Task<OrderReferenceResponse> GetOrderReference(string ueNo)
        {
            OrderReferenceResponse Response = new OrderReferenceResponse();

            var ResponseReference = await _PendingPaymentsContext.GetOrderReferenceNumber(ueNo);

            Response.OrderReferenceNumber = ResponseReference;

            return Response;
        }
        #endregion
    }
}
