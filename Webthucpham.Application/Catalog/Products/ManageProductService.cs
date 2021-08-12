using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Webthucpham.Data.EF;
using System.Linq;
using Webthucpham.Data.Entities;
using Webthucpham.Utilities.Exceptions;
using Microsoft.EntityFrameworkCore;
using Webthucpham.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using Webthucpham.Application.Common;
using Webthucpham.ViewModels.Catalog.Products.Manage;
using Webthucpham.ViewModels.Catalog.Products;

namespace Webthucpham.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService // kế thừa
    {

        private readonly WebthucphamDbContext _context; //đọc dataBase
        private readonly IStorageService _storageService; // đọc
        public ManageProductService(WebthucphamDbContext context, IStorageService storageService)
        {
            _context = context; //gán 1 lần
            _storageService = storageService;
        }

        //thêm ảnh
        public Task<int> AddImages(int productId, List<IFormFile> files)
        {
            throw new NotImplementedException();
        }

        //addView
        public async Task AddViewcount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            product.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        //Tạo sản phẩm
        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
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
           
        
            //Save image

            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Thumbnail image",
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await this.SaveFile(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }
            _context.Products.Add(product);
            return await _context.SaveChangesAsync(); // giúp giảm thời gian chờ
        }
        //delete
        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new WebthucphamException($"Không thể tìm thấy sản phẩm : {productId}");

            var images = _context.ProductImages.Where(i => i.ProductId == productId);
            foreach (var image in images)
            {
                await _storageService.DeleteFileAsync(image.ImagePath);
            }

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();
        }

     
        public async Task<PagedResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request)
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

    
        //List ảnh
        public Task<List<ViewModels.Catalog.Products.ProductImageViewModel>> GetListImage(int productId)
        {
            throw new NotImplementedException();
        }


        // xóa ảnh
        public Task<int> RemoveImages(int imageId)
        {
            throw new NotImplementedException();
        }

        // update san pham
        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslations = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == request.Id && x.LanguageId==request.LanguageId); // sửa riêng cho ngôn ngữ
            if (product == null || productTranslations==null) throw new WebthucphamException($"Không thể tìm thấy sản phẩm với Id: {request.Id}");

            productTranslations.Name = request.Name;
            productTranslations.SeoAlias = request.SeoAlias;
            productTranslations.SeoDescription = request.SeoDescription;
            productTranslations.SeoTitle = request.SeoTitle;
            productTranslations.Description = request.Description;
            productTranslations.Details = request.Details;

            //Save image
            if (request.ThumbnailImage != null)
            {
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault == true && i.ProductId == request.Id);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    _context.ProductImages.Update(thumbnailImage);
                }
            }

            return await _context.SaveChangesAsync();
        }


        // cập nhập ảnh
        public Task<int> UpdateImages(int imageId, string caption, bool isDefault)
        {
            throw new NotImplementedException();
        }


        // cập nhập giá
        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null ) throw new WebthucphamException($"Không thể tìm thấy sản phẩm với Id: {productId}");
            product.Price = newPrice;
            return await _context.SaveChangesAsync()>0;
        }

        // cập nhập số lượng
        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new WebthucphamException($"Không thể tìm thấy sản phẩm với Id: {productId}");
            product.Stock += addedQuantity;
            return await _context.SaveChangesAsync() > 0;
        }

        Task<List<ProductImageViewModel>> IManageProductService.GetListImage(int productId)
        {
            throw new NotImplementedException();
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }
    }
}
