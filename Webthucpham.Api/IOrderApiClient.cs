using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Api
{
    public interface IOrderApiClient
    {
        Task<PageResponse<OrderViewModel>> GetAll(GetOrderRequest request, string status);
        Task<OrderViewModel> GetById(int id);
        Task<bool> UpdateStatus(OrderViewModel request);
        Task<List<OrderProductViewModel>> GetProducts(int id);
    }
}
