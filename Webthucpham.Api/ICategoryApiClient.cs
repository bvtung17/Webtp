using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Categories;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Api
{
    public interface ICategoryApiClient
    {
        Task<List<CategoryViewModel>> GetAll();
        Task<int> Create(CategoryCreateRequest request);
        Task<bool> Edit(CategoryUpdateRequest request);
        Task<bool> Delete(int id);
        Task<List<HomeCategoryViewModel>> GetHomeProductCategories();

        Task<CategoryViewModel> GetById(int id);
        Task<PageResponse<ProductViewModel>> GetProductInCategory(PaginateRequest request, int categoryId);
        Task<PageResponse<CategoryViewModel>> GetAllPaging(PaginateRequest request, string status);
        Task<PageResponse<ProductViewModel>> Search(PaginateRequest request, string categoryId);
    }
}   
 