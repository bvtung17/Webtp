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
using Webthucpham.Data.Enums;


namespace Webthucpham.Application.Catalog.Products
{
    public class ProductService : IProductService // kế thừa
    {
        private readonly WebthucphamDbContext _context;
        private readonly IStorageService _storageService;
        public ProductService(WebthucphamDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

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

        public async Task<ApiResult<bool>> CategoryAssign(CategoryAssignRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            if (product == null)
            {
                return new ApiErrorResult<bool>($"Cannot find product with id: {request.Id}");
            }
            var categories = await _context.Categories.Select(x => x.Id.ToString()).ToArrayAsync();

            foreach (var categoryId in categories)
            {
                var productInCategory = await _context.ProductInCategories.Where(x => x.CategoryId == int.Parse(categoryId) && x.ProductId == product.Id).FirstOrDefaultAsync();
                var isAdd = request.SelectedCategories.Contains(categoryId);
                if (!isAdd && productInCategory != null)
                {
                    _context.ProductInCategories.Remove(productInCategory);
                }
                else if (isAdd && productInCategory == null)
                {
                    await _context.ProductInCategories.AddAsync(new ProductInCategory()
                    {
                        CategoryId = int.Parse(categoryId),
                        ProductId = request.Id
                    });
                }
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product()
            {
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

            if (request.SelectedId != null)
            {
                for (int count = 0; count < request.SelectedId.Length; count++)
                {
                    _context.ProductInCategories.Add(new ProductInCategory()
                    {
                        CategoryId = int.Parse(request.SelectedId[count]),
                        ProductId = product.Id
                    });
                    await _context.SaveChangesAsync();
                }
            }
            return product.Id;

        }

        public async Task<int?> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return null;
            }

            /*var images = await _context.ProductImages.Where(x => x.IsDefault == true && x.ProductId == productId).ToListAsync();

            foreach (var item in images)
            {
                await _storageService.DeleteFileAsync(item.ImagePath);
            }*/
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

        public async Task<PageResponse<ProductViewModel>> GetAll(GetProductRequest request, string status)
        {
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
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        where pi.IsDefault
                        select new { p, pic, c, pi };

            query = query.Where(p => p.pic.CategoryId == request.CategoryId);
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

                var pic = _context.ProductInCategories.Where(x => x.CategoryId == request.CategoryId);
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

        public async Task<ProductUpdateRequest> GetById(int id)
        {

            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return null;
            }
            var queryCtgs = from pic in _context.ProductInCategories
                            join c in _context.Categories on pic.CategoryId equals c.Id
                            select new { pic, c };
            var categories = await queryCtgs.Where(x => x.pic.ProductId == id).Select(x => x.c).ToListAsync() as IEnumerable<Category>;

            var categoryIds = await GetProductInCategories(id);

            var productViewModel = new ProductUpdateRequest()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                Description = product.Description,
                Details = product.Details,
               
                Name = product.Name,
                OriginalCountry = product.OriginalCountry,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                Categories = categoryIds,
                CategoryList = categories,
                status = product.status
            };
            return productViewModel;
        }
        private async Task<List<string>> GetProductInCategories(int id)
        {
            var categoryNames = await (from c in _context.Categories
                                       join pic in _context.ProductInCategories on c.Id equals pic.CategoryId
                                       where pic.ProductId == id
                                       select c.Id.ToString()).ToListAsync();
            return categoryNames;

        }
        public async Task<List<ProductViewModel>> GetFeaturedProducts()
        {
            // Update when have manage categories
            var products = await _context.Products.Where(x => x.Id > 0).Take(8).OrderByDescending(x => x.DateCreated).Select(x => new ProductViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                OriginalPrice = x.OriginalPrice

            }).ToListAsync();

            return products;
        }

        public async Task<ProductImageViewModel> GetImageById(int id)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(x => x.Id == id);
            if (image == null) return null;
            var productImageViewModel = new ProductImageViewModel()
            {
                Id = image.Id,
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                ImagePath = image.ImagePath,
                IsDefault = image.IsDefault,
                ProductId = image.ProductId,
                SortOrder = image.SortOrder
            };
            return productImageViewModel;
        }

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
        public async Task<PageResponse<ProductImageViewModel>> GetImages(int productId, PagingRequestBase request)
        {

            var product = await _context.Products.Where(x => x.Id == productId).FirstOrDefaultAsync();

            if (product == null)
            {
                return null;
            }

            var query = from pi in _context.ProductImages where pi.ProductId == productId select pi;
            var records = await query.CountAsync();
            var images = await query.Select(x => new ProductImageViewModel()
            {
                Id = x.Id,
                Caption = x.Caption,
                DateCreated = x.DateCreated,
                FileSize = x.FileSize,
                ImagePath = x.ImagePath,
                IsDefault = x.IsDefault,
                ProductId = x.ProductId,
                SortOrder = x.SortOrder
            }).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
            var response = new PageResponse<ProductImageViewModel>()
            {
                Items = images,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalRecords = records
            };
            return response;
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

        public async Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {

            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null)
            {
                return 0;
            }
            if (request.ImageFile != null)
            {
                productImage.ImagePath = await SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            productImage.Caption = request.Caption;
            _context.ProductImages.Update(productImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool?> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
            if (product == null)
                return null;
            product.Price = newPrice;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool?> UpdateStock(int productId, int addedStock)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
            if (product == null)
                return null;
            product.Stock += addedStock;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool?> UpdateViewCount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return null;
            product.ViewCount += 1;
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
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

        public async Task<ApiResult<ClientProductViewModel>> ClientGetProductDetail(int id)
        {
            var product = await _context.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return new ApiErrorResult<ClientProductViewModel>("Sản phẩm không tồn tại");
            }

            var clientProduct = new ClientProductViewModel()
            {
                Id = id,
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                Images = await _context.ProductImages.Where(x => x.ProductId == id).Select(pi => pi.ImagePath).ToListAsync()
            };

            return new ApiSuccessResult<ClientProductViewModel>(clientProduct);
        }
    }
}
