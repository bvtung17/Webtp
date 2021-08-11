using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Application.Catalog.Products.Dtos;
using Webthucpham.Application.Dtos;

namespace Webthucpham.Application.Catalog.Products
{
    public interface IManageProductService //quan ly
    {
        Task<int> Create(ProductCreateRequest request);
        Task<int> Update(ProductEditRequest request);
        Task<int> Delete(int productId);
        Task<List<ProductViewModel>> GetAll();
        Task<PagedViewModel<ProductViewModel>> GetAllPaging(string keyword, int pageIndex, int pageSize);

    }
}
