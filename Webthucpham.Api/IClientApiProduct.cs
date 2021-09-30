using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Api
{
    public interface IClientApiProduct
    {
        Task<ApiResult<ClientProductViewModel>> GetProuctDetail(int id);

    }
}
