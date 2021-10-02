using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.Sales;

namespace Webthucpham.Application.Catalog.Orders
{
    public interface IOrderService
    {
        Task<int> Create(OrderCreateRequest request);

        Task<int> Delete(int orderId);

        OrderViewModel GetById(OrderViewModel request);

        Task<bool> UpdateStatus(int orderId, int status);

        Task<PagedResult<OrderViewModel>> GetAllPaging(OrderPagingRequest request);

        Task<PagedResult<OrderViewModel>> GetChart(OrderPagingRequest request);
    }
}
