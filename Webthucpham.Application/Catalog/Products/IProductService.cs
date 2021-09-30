using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.ProductImages;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Catalog.Products.Manage;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Application.Catalog.Products
{
    public interface IProductService 
    {
        Task<int> Create(ProductCreateRequest request);
        Task<int> Update(ProductUpdateRequest request);
        Task<int?> Delete(int productId);
        //Task<PageResponse<ProductViewModel>> GetAllPaging(GetProductRequest query);
        Task<bool?> UpdatePrice(int productId, decimal newPrice);
        Task<bool?> UpdateViewCount(int productId);
        Task<bool?> UpdateStock(int productId, int addedStock);
        Task<ProductUpdateRequest> GetById(int id);
        Task<int> AddImage(int productId, ProductImageCreateRequest request);
        Task<ApiResult<bool>> RemoveImage(int productId, int imageId);
        Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request);
        Task<List<ProductImageViewModel>> GetListImage(int productId);
        Task<ProductImageViewModel> GetImageById(int id);

        Task<PageResponse<ProductViewModel>> GetAll(GetProductRequest request, string status);
        Task<ApiResult<bool>> CategoryAssign(CategoryAssignRequest request);
        Task<List<ProductViewModel>> GetFeaturedProducts();
        Task<PageResponse<ProductImageViewModel>> GetImages(int productId, PagingRequestBase request);
        Task<bool> ChangeThumbnail(int productId, int imageId);
        Task<ApiResult<ClientProductViewModel>> ClientGetProductDetail(int id);

        Task<PageResponse<ProductViewModel>> SearchProductClient(GetProductRequest request);


    }
}
