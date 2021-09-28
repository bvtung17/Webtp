using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Api
{
    public interface IProductApiClient
    {
        Task<PageResponse<ProductViewModel>> GetPagings(GetManageProductPagingRequest request);

        Task<bool> CreateProduct(ProductCreateRequest request);

        Task<bool> UpdateProduct(ProductUpdateRequest request);

        Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request);

        Task<ProductViewModel> GetById(int id, string languageId);
        Task<List<ProductViewModel>> GetFeaturedProducts(string languageId, int take);
        Task<List<ProductViewModel>> GetLatestProducts(string languageId, int take);

        Task<bool> DeleteProduct(int id);

    }
}
