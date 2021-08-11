using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Application.Catalog.Products.Dtos;
using Webthucpham.Application.Catalog.Products.Dtos.Manage;
using Webthucpham.Application.Dtos;

namespace Webthucpham.Application.Catalog.Products
{
    public interface IManageProductService //quan ly
    {
        Task<int> Create(ProductCreateRequest request);
        Task<int> Update(ProductUpdateRequest request);
        Task<int> Delete(int productId);

        Task<bool> UpdatePrice(int productId, decimal newPrice);

        Task<bool> UpdateStock(int productId, int addedQuantity);

        Task AddViewcount(int productId);

        Task<List<ProductViewModel>> GetAll();
        Task<PagedResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request);

    }
}
