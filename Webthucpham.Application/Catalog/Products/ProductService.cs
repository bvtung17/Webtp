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
using Webthucpham.ViewModels.Catalog.ProductImages;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.Utilities.Constants;
using Webthucpham.Data.Enums;
using Webthucpham.ViewModels.Catalog.Products.Manage;

namespace Webthucpham.Application.Catalog.Products
{
    public class ProductService : IProductService // kế thừa
    {

        private readonly WebthucphamDbContext _context; //đọc dataBase
        private readonly IStorageService _storageService; // đọc
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        public ProductService(WebthucphamDbContext context, IStorageService storageService)
        {
            _context = context; //gán 1 lần
            _storageService = storageService;
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
                 CategoryId= request.CategoryId,
                 Price = request.Price,
                 Name = request.Name,
                 OriginalCountry = request.OriginalCountry,
                 OriginalPrice = request.OriginalPrice,
                 Stock = request.Stock,
                 Description = request.Description ?? "",
                 Details = request.Details ?? "",
                 ViewCount = 0,
                 DateCreated = DateTime.Now,
                 status = Status.Active
             };
            //Save image

            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption= "Thumbnail image",
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        IsDefault = true,
                        SortOrder = 1,
                        ImagePath = await this.SaveFile(request.ThumbnailImage)
                    }
                };
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
   
           
        }
        //XÓA SẢN PHẨM
        public async Task<int?> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new WebthucphamException($"Không thể tìm thấy sản phẩm : {productId}");

            if (product.status == Status.Active)
            {
                product.status = Status.InActive;
            }
            else
            {
                product.status = Status.Active;
            }
            _context.Products.Update(product);
            return await _context.SaveChangesAsync();
        }

        //GET ALL
        public async Task<PageResponse<ProductViewModel>> GetAll(GetProductRequest request, string status)
        {
            //using linq
            int PageIndex = request.PageIndex;
            int PageSize = request.PageSize;
            var totalRecords = 0;


            if (request.CategoryId == null)
            {
                var result = from p in _context.Products
                             join pi in _context.ProductImages on p.Id equals pi.ProductId
                             where pi.IsDefault
                             select new { p, pi };

                if (request.Keyword != null)
                {
                    result = result.Where(x => x.p.Name.Contains(request.Keyword));

                }
                switch (status)
                {
                    case "InActive":
                        result = result.Where(x => x.p.status == Status.InActive);
                        break;
                    case "Active":
                        result = result.Where(x => x.p.status == Status.Active);
                        break;
                    default:
                        /*result = result.Where(x => x.p.status == Status.Active);*/
                        break;
                }
                totalRecords = await result.CountAsync();
                var products = await result.Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new ProductViewModel()
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
                    status = x.p.status,
                    ViewCount = x.p.ViewCount,
                    ImagePath = x.pi.ImagePath
                }).ToListAsync();
                var responseData = new PageResponse<ProductViewModel>()
                {
                    Items = products,
                    TotalRecords = totalRecords,
                    PageIndex = PageIndex,
                    PageSize = PageSize
                };
                return responseData;
            }
            var query = from p in _context.Products
                        join c in _context.Categories on p.CategoryId equals c.Id
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        where pi.IsDefault
                        select new { p, c, pi };
            query = query.Where(p => p.c.Id == request.CategoryId);
            switch (status)
            {
                case "InActive":
                    query = query.Where(x => x.p.status == Status.InActive);
                    break;
                case "Active":
                    query = query.Where(x => x.p.status == Status.Active);
                    break;
                default:
                    /*query = query.Where(x => x.p.status == Status.Active);*/
                    break;
            }
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
                })
                .ToListAsync();

            var response = new PageResponse<ProductViewModel>()
            {
                Items = data,
                TotalRecords = totalRecords,
                PageIndex = PageIndex,
                PageSize = PageSize
            };

            return response;

        }

        //Lay id san pham
        public async Task<ProductUpdateRequest> GetById(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return null;
            }
            //var queryCtgs = from p in _context.Products
            //                join c in _context.Categories on p.CategoryId equals c.Id
            //                select new { p, c };
            //var categoryId =  queryCtgs.Where(x => x.p.Id == id).Select(x => x.c.Id);
            var productViewModel = new ProductUpdateRequest()
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                DateCreated = product.DateCreated,
                Description = product.Description,
                Details = product.Details,
                Name = product.Name,
                OriginalCountry = product.OriginalCountry,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                status = product.status
            };
            return productViewModel;

        }

        //private async Task<List<string>> GetProductInCategories(int id)
        //{
        //    var categoryNames = await (from c in _context.Categories
        //                               join pic in _context.ProductInCategories on c.Id equals pic.CategoryId
        //                               where pic.ProductId == id
        //                               select c.Id.ToString()).ToListAsync();
        //    return categoryNames;

        //}

        // update san pham
        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (product == null)
                throw new WebthucphamException($"Cannot found product width id: {request.Id}");

            product.Name = request.Name;
            product.OriginalCountry = request.OriginalCountry;
           
            product.Description = request.Description;
            product.Details = request.Details;
            product.OriginalPrice = request.OriginalPrice;
            product.Price = request.Price;
            product.Stock = request.Stock;

            _context.Products.Attach(product);

            if (request.ThumbnailImage != null)
            {
                var thumbnail = await _context.ProductImages.FirstOrDefaultAsync(x => x.IsDefault == true && x.ProductId == request.Id);
                if (thumbnail != null)
                {
                    thumbnail.FileSize = request.ThumbnailImage.Length;
                    thumbnail.ImagePath = await SaveFile(request.ThumbnailImage);
                    _context.ProductImages.Update(thumbnail);
                }
            }
            await _context.SaveChangesAsync();
            return 1 /*1 is success*/;

        }

        // cập nhập giá
        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new WebthucphamException($"Không thể tìm thấy sản phẩm với Id: {productId}");
            product.Price = newPrice;
            return await _context.SaveChangesAsync() > 0;
        }

        // cập nhập số lượng
        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new WebthucphamException($"Không thể tìm thấy sản phẩm với Id: {productId}");
            product.Stock += addedQuantity;
            return await _context.SaveChangesAsync() > 0;
        }

        //IMAGES

        // GET ID ẢNH
        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
            {
                throw new WebthucphamException($"Không tìm thấy ảnh theo id {imageId}");
            }
            var viewModel = new ProductImageViewModel()
            {
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                Id = image.Id,
                ImagePath = image.ImagePath,
                IsDefault = image.IsDefault,
                ProductId = image.ProductId,
                SortOrder = image.SortOrder
            };
            return viewModel;

        }
        //THÊM ẢNH
        public async Task<int> AddImage(int productId, ProductImageCreateRequest request)
        {

            if (request.ImageFile == null)
            {
                throw new WebthucphamException("Error");
            }
            var image = new ProductImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                FileSize = request.ImageFile.Length,
                ImagePath = await SaveFile(request.ImageFile),
                ProductId = productId,
            };
            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();
            return image.Id;
        }
        //lấy list ảnh
        public async Task<List<ProductImageViewModel>> GetListImage(int productId)
        {
            var images = await _context.ProductImages
                .Where(x => x.ProductId == productId)
                .Select(x => new ProductImageViewModel()
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Caption = x.Caption,
                    DateCreated = x.DateCreated,
                    FileSize = x.FileSize,
                    ImagePath = x.ImagePath,
                    IsDefault = x.IsDefault,
                    SortOrder = x.SortOrder
                }
                )
                .ToListAsync();
            return images;
        }


        //CẬP NHẬP ẢNH
        public async Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null)
            {
                throw new WebthucphamException($"Không tìm thấy ảnh theo id {imageId}");
            }
            //Save image
            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _context.ProductImages.Update(productImage);
            return await _context.SaveChangesAsync();
        }
        //XÓA ẢNH
        public async Task<int> RemoveImage( int imageId)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null)
            {
                throw new WebthucphamException($"Không tìm thấy ảnh theo id {imageId}");
            }
            _context.ProductImages.Remove(productImage);
           return await _context.SaveChangesAsync();
        }
        //LƯU FILE
        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return "/"+ USER_CONTENT_FOLDER_NAME+"/"+fileName;
        }
        public async Task<List<ProductViewModel>> GetFeaturedProducts()
        {

            //1 : select join
            var products = await _context.Products.Where(x => x.Id > 0).Take(8).OrderByDescending(x => x.DateCreated).Select(x => new ProductViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                OriginalPrice = x.OriginalPrice

            }).ToListAsync();

            return products;
        }

      
        public async Task<bool?> UpdateViewCount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return null;
            product.ViewCount += 1;
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool?> UpdateStock(ProductEditRequest request)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (product == null)
                return null;
            product.Stock += request.addedStock;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ApiResult<bool>> RemoveImage(int productId, int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            var product = await _context.Products.FindAsync(productId);
            if (image == null || product == null) return new ApiErrorResult<bool>("Không tìm thấy image");

            var countProductImage = await _context.ProductImages.Where(x => x.ProductId == productId).CountAsync();

            if (countProductImage > 1 && image.IsDefault)
            {
                var ThumbnailNew = await _context.ProductImages.Where(x => x.Id != imageId && x.ProductId == productId).FirstOrDefaultAsync();
                ThumbnailNew.IsDefault = true;
                _context.ProductImages.Attach(ThumbnailNew);
            }

            if (countProductImage == 1)
            {

                return new ApiSuccessResult<bool>() { IsSuccessed = true, ResultObj = false, Message = "Không thể xóa hết ảnh của sản phẩm!" };
            }

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>()
            {
                IsSuccessed = true,
                Message = "Xóa thành công",
                ResultObj = true
            };
        }

        public Task<ApiResult<bool>> CategoryAssign(CategoryAssignRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PageResponse<ProductImageViewModel>> GetImages(int productId, PagingRequestBase request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<ClientProductViewModel>> ClientGetProductDetail(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PageResponse<ProductViewModel>> SearchProductClient(GetProductRequest request)
        {
            int PageIndex = request.PageIndex;
            int PageSize = 8;
            var totalRecords = 0;

            if (request.CategoryId == null)
            {
                var result = from p in _context.Products
                             join pi in _context.ProductImages on p.Id equals pi.ProductId
                             where pi.IsDefault
                             select new { p, pi };

                result = result.Where(x => x.p.status == Status.Active);
                totalRecords = await result.CountAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    result = result.Where(x => x.p.Name.Contains(request.Keyword));
                }
                if ((request.PriceStart).HasValue && (request.PriceEnd).HasValue)
                {
                    result = result.Where(x => x.p.Price >= request.PriceStart && x.p.Price <= request.PriceEnd);
                }
                if (request.SortPrice == "2")
                {
                    result = result.OrderByDescending(x => x.p.Price);
                }
                else if (request.SortPrice == "1")
                {
                    result = result.OrderBy(x => x.p.Price);
                }
                else
                {
                }
                totalRecords = await result.Select(x => x.p).CountAsync();

                var products = await result.Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new ProductViewModel()
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
                var responseData = new PageResponse<ProductViewModel>()
                {
                    Items = products,
                    TotalRecords = totalRecords,
                    PageIndex = PageIndex,
                    PageSize = PageSize
                };

                return responseData;

            }
            else
            {
                var Category = await _context.Categories.Where(x => x.Id == request.CategoryId).FirstOrDefaultAsync();

                var pic = _context.Products.Where(x => x.CategoryId == request.CategoryId);
                var query = from pc in pic
                            join p in _context.Products on pc.Id equals p.Id
                            join pi in _context.ProductImages on p.Id equals pi.ProductId
                            where pi.IsDefault
                            select new { p, pi };

                query = query.Where(x => x.p.status == Status.Active);
                totalRecords = await query.CountAsync();
                if (request.Keyword != null)
                {
                    query = query.Where(x => x.p.Name.Contains(request.Keyword));

                }
                if ((request.PriceStart).HasValue && (request.PriceEnd).HasValue)
                {
                    query = query.Where(x => x.p.Price >= request.PriceStart && x.p.Price <= request.PriceEnd);
                }
                if (request.SortPrice == "2")
                {
                    query = query.OrderByDescending(x => x.p.Price);
                }
                else if (request.SortPrice == "1")
                {
                    query = query.OrderBy(x => x.p.Price);
                }
                else
                {
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

        public async Task<bool> ChangeThumbnail(int productId, int imageId)
            {
            var product = await _context.Products.Where(x => x.Id == productId).FirstOrDefaultAsync();

            if (product == null) return false;


            var image = await _context.ProductImages.Where(x => x.Id == imageId && x.ProductId == productId).FirstOrDefaultAsync();

            if (image == null) return false;
            var thumbnail = await _context.ProductImages.Where(x => x.IsDefault && x.Id != imageId).FirstOrDefaultAsync();

            if (thumbnail == null)
            {
                thumbnail.IsDefault = true;
            }
            else
            {
                thumbnail.IsDefault = false;
            }
            image.IsDefault = true;
            _context.ProductImages.Attach(image);
            _context.ProductImages.Attach(thumbnail);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool?> UpdatePrice(ProductEditRequest request)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (product == null)
                return null;
            product.Price = request.newPrice;
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool?> UpdateViewCount(ProductEditRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            if (product == null) return null;
            product.ViewCount += 1;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
