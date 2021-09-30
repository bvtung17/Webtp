using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Data.EF;
using Webthucpham.ViewModels.Catalog.Categories;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Webthucpham.Data.Enums;
using Webthucpham.ViewModels.Common;
using Webthucpham.Data.Entities;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Catalog.ProductImages;

namespace Webthucpham.Application.Catalog.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly WebthucphamDbContext _context;

        public CategoryService(WebthucphamDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(CategoryCreateRequest request)
        {
            var category = new Category()
            {
                Name = request.Name,
                SortOrder = request.SortOrder,
                Status = Status.Active,
                CreatedDate = DateTime.Now
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }
            if (category.Status == Status.Active)
            {
                category.Status = Status.InActive;
            }
            else
            {
                category.Status = Status.Active;
            }
            _context.Categories.Update(category);

            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> Edit(CategoryUpdateRequest request)
        {
            var category = await _context.Categories.FindAsync(request.Id);
            if (category == null)
            {
                return false;
            }

            category.Name = request.Name;
            category.IsOutstanding = request.IsOutstanding;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CategoryViewModel>> GetAll()
        {
            var query = from c in _context.Categories select c;
            query = query.Where(x => x.Status == Status.Active);
            var categories = await query.Select(x => new CategoryViewModel()
            {
                Id = x.Id,
                IsOutstanding = x.IsOutstanding,
                Name = x.Name,
                ParentId = x.ParentId,
                SortOrder = x.SortOrder,
                Status = x.Status
            }).ToListAsync();

            return categories;
        }

        public async Task<PageResponse<CategoryViewModel>> GetAllPaging(PaginateRequest request, string status)
        {
            int PageIndex = request.PageIndex;
            int PageSize = request.PageSize;
            var totalRecords = 0;


            var query = from c in _context.Categories select c;

            if (request.Keyword != null)
            {
                query = query.Where(x => x.Name.Contains(request.Keyword));

            }
            switch (status)
            {
                case "InActive":
                    query = query.Where(x => x.Status == Status.InActive);
                    break;
                case "Active":
                    query = query.Where(x => x.Status == Status.Active);
                    break;
                default:
                    query = query.Where(x => x.Status == Status.Active);
                    break;
            }
            totalRecords = await query.CountAsync();

            var data = await query.Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .Select(x => new CategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsOutstanding = x.IsOutstanding,
                    Status = x.Status

                })
                .ToListAsync();

            var response = new PageResponse<CategoryViewModel>()
            {
                Items = data,
                TotalRecords = totalRecords,
                PageIndex = PageIndex,
                PageSize = PageSize
            };

            return response;
        }

        public async Task<CategoryViewModel> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;
            var categoryViewModel = new CategoryViewModel()
            {
                Id = category.Id,
                IsOutstanding = category.IsOutstanding,
                Name = category.Name,
                SortOrder = category.SortOrder
            };
            return categoryViewModel;
        }

        public async Task<List<HomeCategoryViewModel>> GetProductCategories()
        {
            var topCategories = await _context.Categories.Where(x => x.IsOutstanding).ToListAsync();
            var listHomeCategory = new List<HomeCategoryViewModel>();
            foreach (var category in topCategories)
            {
                if (category == null) continue;
                var pic = await _context.ProductInCategories.Where(x => x.CategoryId == category.Id).ToListAsync();
                var query = from pc in pic
                            join p in _context.Products on pc.ProductId equals p.Id
                            select p;
                query = query.Where(x => x.status == Status.Active);
                var productsInCategory = query
                    .Select(p => new HomeProductViewModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        OriginalPrice = p.OriginalPrice,
                    }).Take(8).Skip(0).ToList();
                foreach (var product in productsInCategory)
                {
                    var images = await _context.ProductImages.Where(x => x.ProductId == product.Id)
                        .Select(x => new ProductImageViewModel()
                        {
                            Id = x.Id,
                            ProductId = product.Id,
                            Caption = x.Caption,
                            DateCreated = x.DateCreated,
                            FileSize = x.FileSize,
                            ImagePath = x.ImagePath,
                            IsDefault = x.IsDefault,
                            SortOrder = x.SortOrder
                        }).OrderByDescending(x => x.IsDefault)
                        .ToListAsync();
                    product.Images = images;
                }

                listHomeCategory.Add(new HomeCategoryViewModel()
                {
                    CategoryName = category.Name,
                    CategoryId = category.Id,
                    Products = productsInCategory
                });
            }

            return listHomeCategory;
        }


        public async Task<PageResponse<ProductViewModel>> GetProductInCategory(PaginateRequest request, int categoryId)
        {
            int PageIndex = request.PageIndex;
            int PageSize = request.PageSize;
            var totalRecords = 0;

            var Category = await _context.Categories.Where(x => x.Id == categoryId).FirstOrDefaultAsync();

            var pic = _context.ProductInCategories.Where(x => x.CategoryId == categoryId);
            var query = from pc in pic
                        join p in _context.Products on pc.ProductId equals p.Id
                        join pi in _context.ProductImages on p.Id equals pi.ProductId
                        where pi.IsDefault
                        select new { p, pi };

            query = query.Where(x => x.p.status == Status.Active);
            totalRecords = await query.CountAsync();
            if (request.Keyword != null)
            {
                query = query.Where(x => x.p.Name.Contains(request.Keyword));

            }
            totalRecords = await query.Select(x => x.p).CountAsync();

            var data = await query.Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name = x.p.Name,
                    Price = x.p.Price,
                   
                    OriginalPrice = x.p.OriginalPrice,
                    DateCreated = x.p.DateCreated,
                    Description = x.p.Description,
                    Details = x.p.Details,
                    OriginalCountry = x.p.OriginalCountry,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ImagePath = x.pi.ImagePath,
                    status = x.p.status
                }).ToListAsync();

            var response = new PageResponse<ProductViewModel>()
            {
                Items = data,
                TotalRecords = totalRecords,
                PageIndex = PageIndex,
                PageSize = PageSize
            };

            return response;
        }

        //re
        public async Task<PageResponse<ProductViewModel>> SearchProductClient(PaginateRequest request, string categoryId)
        {
            int PageIndex = request.PageIndex;
            int PageSize = request.PageSize;
            var totalRecords = 0;

            var Category = await _context.Categories.Where(x => x.Id == Int16.Parse(categoryId)).FirstOrDefaultAsync();

            var pic = _context.ProductInCategories.Where(x => x.CategoryId == Int16.Parse(categoryId));
            var query = from pc in pic
                        join p in _context.Products on pc.ProductId equals p.Id
                        join pi in _context.ProductImages on p.Id equals pi.ProductId
                        where pi.IsDefault
                        select new { p, pi };

            query = query.Where(x => x.p.status == Status.Active);
            totalRecords = await query.CountAsync();
            if (request.Keyword != null)
            {
                query = query.Where(x => x.p.Name.Contains(request.Keyword));

            }
            totalRecords = await query.Select(x => x.p).CountAsync();

            var data = await query.Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name = x.p.Name,
                    Price = x.p.Price,
                 
                    OriginalPrice = x.p.OriginalPrice,
                    DateCreated = x.p.DateCreated,
                    Description = x.p.Description,
                    Details = x.p.Details,
                    OriginalCountry = x.p.OriginalCountry,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ImagePath = x.pi.ImagePath,
                    status = x.p.status
                }).ToListAsync();

            var response = new PageResponse<ProductViewModel>()
            {
                Items = data,
                TotalRecords = totalRecords,
                PageIndex = PageIndex,
                PageSize = PageSize
            };

            return response;
        }
    }
}

