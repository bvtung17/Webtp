using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Categories;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Application.Catalog.Categories
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetAll();
        Task<PageResponse<CategoryViewModel>> GetAllPaging(PaginateRequest request, string status);
        Task<PageResponse<ProductViewModel>> GetProductInCategory(PaginateRequest request, int categoryId);
        Task<CategoryViewModel> GetById(int id);
        Task<bool> Delete(int id);
        Task<int> Create(CategoryCreateRequest request);
        Task<bool> Edit(CategoryUpdateRequest request);
        Task<List<HomeCategoryViewModel>> GetProductCategories();
    }
}
