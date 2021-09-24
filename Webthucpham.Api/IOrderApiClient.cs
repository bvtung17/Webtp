using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.Sales;

namespace Webthucpham.Api
{
    public interface IOrderApiClient
    {
        Task<PagedResult<OrderViewModel>> GetPagings(OrderPagingRequest request);

        Task<PagedResult<OrderViewModel>> GetChart(OrderPagingRequest request);

        Task<bool> Create(OrderCreateRequest request);

        Task<bool> UpdateStatus(int orderId, int status);

        Task<OrderViewModel> GetById(OrderViewModel request);

        Task<bool> Delete(int id);
    }
}
