using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Application.Catalog.Products
{
    public interface IPublicProductService //Công khai
    {
        Task<PagedResult<ProductViewModel>> GetAllByCategoryId(string languageId,GetPublicProductPagingRequest request);
       
    }
}
