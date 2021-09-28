﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Categories;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Application.Catalog.Categories
{
    public interface ICategoryService
    {
        Task<List<CategoryVm>> GetAll();
        Task<CategoryVm> GetById(int id);
        Task<PageResponse<CategoryVm>> GetAllPaging(PaginateRequest request, string status);
        Task<bool> Delete(int id);
        Task<int> Create(CategoryCreateRequest request);
        Task<bool> Edit(CategoryUpdateRequest request);
        //Task<List<HomeCategoryViewModel>> GetProductCategories();
    }
}
