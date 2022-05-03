using System.Threading.Tasks;

using Soriana.PPS.Common.DTO.OrderData;

namespace Soriana.PPS.Order.GetOrderReferenceNumber.Services
{
    public interface IOrderReferenceService
    {
        Task<OrderReferenceResponse> GetOrderReference(string ueNo);
    }
}
