using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.System.Clients;

namespace Webthucpham.Application.Catalog.Clients
{
    public interface IClientService
    {
        Task<ApiResult<string>> Register(ClientRegisterRequest request);
        Task<ApiResult<string>> Login(ClientLoginRequest request);
        Task<ApiResult<ClientUpdateViewModel>> GetDetail(Guid clientId);
        Task<ApiResult<ClientUpdateViewModel>> Update(ClientUpdateViewModel request);
        Task<ApiResult<PageResponse<ClientViewModel>>> GetClientPaging(GetClientPagingRequest request);
        Task<ClientViewModel> GetClientById(Guid id);
        Task<List<OrderViewModel>> GetOrderByClient(Guid id);
    }
}
