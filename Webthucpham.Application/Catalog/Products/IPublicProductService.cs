using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Application.Catalog.Products.Dtos;
using Webthucpham.Application.Catalog.Products.Dtos.Public;
using Webthucpham.Application.Dtos;

namespace Webthucpham.Application.Catalog.Products
{
    public interface IPublicProductService //Công khai
    {
        Task<PagedResult<ProductViewModel>> GetAllByCategory(GetProductPagingRequest request);
    }
}
