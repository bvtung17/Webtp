using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Carts;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Api
{
    public interface ICartApiClient
    {
        Task<ClientCartViewModel> AddToCart(ClientCartViewModel request);
        Task<ClientCartViewModel> UpdateCart(ClientCartViewModel request);
        Task<ApiResult<ClientCartViewModel>> GetCart(Guid id);
        Task<ApiResult<ClientCartViewModel>> RemoveProduct(DeleteProductInCartRequest request);
    }
}
