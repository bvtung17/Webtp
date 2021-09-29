using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.System.Clients;

namespace Webthucpham.Api
{
    public interface IClientApi
    {
        Task<ApiResult<string>> Register(ClientRegisterRequest request);
        Task<ApiResult<string>> Login(ClientLoginRequest request);
        Task<ApiResult<ClientUpdateViewModel>> GetDetail(Guid userId);
        Task<ApiResult<ClientUpdateViewModel>> Update(ClientUpdateViewModel request);
        Task<ApiResult<PageResponse<ClientViewModel>>> GetClientPaging(GetClientPagingRequest request);

        Task<ClientViewModel> GetByClientId(Guid clientid);
        Task<List<OrderViewModel>> GetOrderByClientId(Guid id);
    }
}
