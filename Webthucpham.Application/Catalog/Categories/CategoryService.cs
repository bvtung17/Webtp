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

namespace Webthucpham.Application.Catalog.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly WebthucphamDbContext _context;

        public CategoryService(WebthucphamDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryViewModel>> GetAll()
        {
            var query = from c in _context.Categories
                        where c.Status == Status.Active
                        select c

                        ;
            var categories = await query.Select(x => new CategoryViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                SortOrder = x.SortOrder,
                IsOutstanding = x.IsOutstanding,
                ParentId = x.ParentId,
                Status = x.Status,
                //Products = x.Products

            }).ToListAsync();

            return categories;
        }

        public async Task<PageResponse<CategoryViewModel>>GetAllPaging(PaginateRequest request, string status)
        {
            int PageIndex = request.PageIndex; //= 1
            int PageSize = request.PageSize; //=5
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
                }).ToListAsync();           
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
            if (category == null)
            {
                return null;
            }
            var categoryVm = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                IsOutstanding = category.IsOutstanding,
                SortOrder = category.SortOrder
            };
            return categoryVm;
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
            if(category.Status == Status.Active)
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
    }
}
