using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.ProductImages;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Catalog.Products.Manage;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Api
{
    public interface IProductApiClient
    {
        Task<PageResponse<ProductViewModel>> GetPaging(GetProductRequest request, string status);
        Task<ProductUpdateRequest> Create(ProductCreateRequest request);
        Task<bool> Update(ProductUpdateRequest request);
        Task<bool> Delete(int product_id);
        Task<ApiResult<bool>> CategoryAssign(CategoryAssignRequest request);
        Task<ProductUpdateRequest> GetById(int id);
        Task<PageResponse<ProductImageViewModel>> GetProductImages(int productId, PagingRequestBase request);
        Task<bool> AddImage(ProductImageCreateRequest request);
        Task<bool> ChangeThumbnail(int imageId, int productId);
        Task<bool> UpdateProductImage(ProductImageUpdateRequest request);
        Task<ProductImageViewModel> GetImageById(int productId, int imageId);
        Task<ApiResult<bool>> DeleteImage(int productId, int imageId);
        Task<PageResponse<ProductViewModel>> SearchProduct(GetProductRequest request);

    }
}
