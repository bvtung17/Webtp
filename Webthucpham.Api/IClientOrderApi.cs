using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Common;
namespace Webthucpham.Api
{
    public interface IClientOrderApi
    {
        Task<int> ClientCreateOrder(ClientCreateOrderViewModel request);
        Task<ApiResult<ClientOrderViewModel>> GetOrder(Guid cartId, int orderId);
        Task<PageResponse<ClientOrderHistoryViewMode>> GetOrderHistory(Guid clientId, GetOrderRequest request, string status);
        Task<OrderViewModel> GetClientOrderDetails(Guid clientId, int orderId);
        Task<ApiResult<bool>> ClientCancelOrder(int orderId, string cancelReason);
    }
}
