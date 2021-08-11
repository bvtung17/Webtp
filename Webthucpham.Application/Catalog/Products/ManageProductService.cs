using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Application.Catalog.Products.Dtos;
using Webthucpham.Application.Catalog.Products.Dtos.Manage;
using Webthucpham.Application.Dtos;
using Webthucpham.Data.EF;
using System.Linq;
using Webthucpham.Data.Entities;
using Webthucpham.Utilities.Exceptions;
using Microsoft.EntityFrameworkCore;


namespace Webthucpham.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService // kế thừa
    {

        private readonly WebthucphamDbContext _context; //đọc
        public ManageProductService(WebthucphamDbContext context)
        {
            _context = context; //gán 1 lần
        }

        public async Task AddViewcount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            product.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount =0,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation()
                    { 
                        Name = request.Name,
                        Description = request.Description,
                        Details = request.Details,
                        SeoDescription= request.SeoDescription,
                        SeoAlias = request.SeoAlias,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId,

                       
                    }

                }
            };
            _context.Products.Add(product);
            return await _context.SaveChangesAsync(); // giúp giảm thời gian chờ
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new WebthucphamException($"Không thể tìm thấy sản phẩm : {productId}");

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();
        }

        public Task<List<ProductViewModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request)
        {
            //using linq

            //1 : select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId // với bảng Translation
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId //với bảng ProcutInCategory
                        join c in _context.Categories on pic.CategoryId equals c.Id           // với bảng Category
                        select new { p, pt ,pic }; 
            
            // 2 : filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));
            if (request.CategoryIds.Count >0)
            {
                query = query.Where(p=> request.CategoryIds.Contains(p.pic.CategoryId));
            }
            //3 : Paging
            int totalRow = await query.CountAsync();
                //add data
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount

                }).ToListAsync();

            // 4 :  Select and Project
            var pagedResult = new PagedResult<ProductViewModel>
            {
                TotalRecord = totalRow,
                Items = data
            };
            return pagedResult;


        }

        public Task<int> Update(ProductUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            throw new NotImplementedException();
        }
    }
}
