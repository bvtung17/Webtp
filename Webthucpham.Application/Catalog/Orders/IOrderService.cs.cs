using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.Sales;

namespace Webthucpham.Application.Catalog.Orders
{
    public interface IOrderService
    {
        Task<PageResponse<OrderViewModel>> GetAll(GetOrderRequest request, string status);
        Task<OrderViewModel> GetById(int id);
        Task<OrderViewModel> GetclientOrderDetails(Guid clientId, int id);
        Task<bool> UpdateStatus(OrderViewModel request);
        Task<List<OrderProductViewModel>> GetOrderProducts(int orderId);
        Task<int> ClientCreateOrder(ClientCreateOrderViewModel request);
        Task<ApiResult<ClientOrderViewModel>> ClientGetOrder(Guid cartId, int orderId);
        Task<PageResponse<ClientOrderHistoryViewMode>> ClientGetOrderHistory(Guid clientId, GetOrderRequest request, string status);
        Task<ApiResult<bool>> ClientCancelOrder(int orderId, string reason);
    }
}
