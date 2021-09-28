using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Carts;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Application.Catalog.Carts
{
    public interface ICartService
    {
        Task<ClientCartViewModel> AddToCart(ClientCartViewModel request);
        Task<ClientCartViewModel> UpdateCart(ClientCartViewModel request);
        Task<ApiResult<ClientCartViewModel>> GetClientCart(Guid id);
        Task<ApiResult<ClientCartViewModel>> RemoveProductInCart(DeleteProductInCartRequest request);
    }
}
