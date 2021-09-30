using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Webthucpham.Api;
using Webthucpham.Data.Enums;
using Webthucpham.Utilities.Constants;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.AdminApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly IConfiguration _config;
        private readonly ICategoryApiClient _categoryApiClient;
        public ProductController(IProductApiClient productApiClient,
            IConfiguration config,
            ICategoryApiClient categoryApiClient)
        {
            _config = config;
            _productApiClient = productApiClient;
            _categoryApiClient = categoryApiClient;
        }


        public async Task<IActionResult> Index(GetProductRequest request, string status)
        {

            var data = await _productApiClient.GetPaging(request, status);
            ViewBag.Keyword = request.Keyword;
            var categories = await _categoryApiClient.GetAll();
            if (categories != null)
            {
                ViewBag.Categories = categories.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = request.CategoryId.HasValue && request.CategoryId.Value == x.Id
                });
            }

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await GetCategoryAssignRequest();
            ProductCreateRequest productCreateRequest = new ProductCreateRequest()
            {
                CategoriesAssignRequest = categories
            };
            return View(productCreateRequest);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            var categories = await GetCategoryAssignRequest();

            if (!ModelState.IsValid)
            {
                request.CategoriesAssignRequest = categories;
                return View(request);
            }

            var product = await _productApiClient.Create(request);

            if (product != null)
            {
                var categoryRequest = new CategoryAssignRequest()
                {
                    Categories = categories,
                    Id = product.Id,
                    SelectedCategories = request.SelectedId
                };

                var categoryAssignResult = await _productApiClient.CategoryAssign(categoryRequest);
                if (categoryAssignResult.IsSuccessed)
                {
                    TempData["result"] = "Tạo sản phẩm thành công!";
                    return RedirectToAction("Index");
                }

            }
            ModelState.AddModelError("", "Tạo sản phẩm thất bại!");
            request.CategoriesAssignRequest = categories;
            return View(request);
        }

        [HttpPost]
        public async Task Delete(int id)
        {
            if (id > 0)
            {
                var result = await _productApiClient.Delete(id);
                var product = await _productApiClient.GetById(id);

                if (product.status != Status.Active)
                {
                    if (result)
                    {
                        TempData["result"] = "Xóa thành công";
                        RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "Xóa thất bại";
                        RedirectToAction("Index");
                    }
                }
                else
                {
                    if (result)
                    {
                        TempData["result"] = "Kích hoạt thành công";
                        RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "kích hoạt thất bại";
                        RedirectToAction("Index");
                    }
                }
            }
            RedirectToAction("Error", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var categoryAssignRequest = await GetCategoryAssignRequest(id);
            var product = await _productApiClient.GetById(id);
            product.CategoriesAssignRequest = categoryAssignRequest.Categories;
            product.SelectedId = categoryAssignRequest.SelectedCategories;
            return View(product);
        }


        [HttpPut("product/{productId}/images/{imageId}/thumbnail")]
        public async Task<IActionResult> ChangeThumbnail(int imageId, int productId)
        {
            if (imageId > 0)
            {
                var result = await _productApiClient.ChangeThumbnail(imageId, productId);
                if (result)
                    TempData["result"] = "Cập nhật thành công!";
                return Redirect("/");
            }

            return RedirectToAction("Error", "Home");
        }

        [HttpGet("product/{id}/images")]
        public async Task<IActionResult> Images(int id, [FromQuery] PagingRequestBase request)
        {

            if (!ModelState.IsValid || id == 0)
            {
                return RedirectToAction("Error", "Home");
            }

            var images = await _productApiClient.GetProductImages(id, request);
            ViewBag.ProductId = id;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            if (TempData["error"] != null)
            {
                ViewBag.ErrorMsg = TempData["error"];
            }
            return View(images);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            var categoryAssignRequest = await GetCategoryAssignRequest(request.Id);
            if (!ModelState.IsValid)
            {
                request.CategoriesAssignRequest = categoryAssignRequest.Categories;
                ModelState.AddModelError("", "Cập nhập thông tin sản phẩm không thành công!");
                return View(request);
            }

            var result = await _productApiClient.Update(request);
            if (result)
            {
                var categoryRequest = new CategoryAssignRequest()
                {
                    Categories = request.CategoriesAssignRequest,
                    Id = request.Id,
                    SelectedCategories = request.SelectedId
                };

                var categoryAssignResult = await _productApiClient.CategoryAssign(categoryRequest);
                if (categoryAssignResult.IsSuccessed)
                {
                    TempData["result"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction("Index");
                }

            }
            request.CategoriesAssignRequest = categoryAssignRequest.Categories;
            ModelState.AddModelError("", "Cập nhật thông tin sản phẩm thất bại!");
            return View(request);
        }


        private async Task<CategoryAssignRequest> GetCategoryAssignRequest(int id)
        {
            var productViewModel = await _productApiClient.GetById(id);
            var categories = await _categoryApiClient.GetAll();
            var categoryAssignRequest = new CategoryAssignRequest();
            categoryAssignRequest.Id = id;
            List<string> selectedCategories = new List<string>();
            foreach (var category in categories)
            {
                categoryAssignRequest.Categories.Add(new SelectItemDynamic<int>()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Selected = productViewModel.Categories.Contains(category.Id.ToString())
                });
                if (productViewModel.Categories.Contains(category.Id.ToString()))
                {
                    selectedCategories.Add(category.Id.ToString());
                }
            }
            categoryAssignRequest.SelectedCategories = selectedCategories.ToArray();
            return categoryAssignRequest;
        }
        private async Task<List<SelectItemDynamic<int>>> GetCategoryAssignRequest()
        {
            var categories = await _categoryApiClient.GetAll();
            var categoryAssignRequest = new CategoryAssignRequest();
            foreach (var category in categories)
            {
                categoryAssignRequest.Categories.Add(new SelectItemDynamic<int>()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Selected = false
                });

            }
            return categoryAssignRequest.Categories;
        }
    }
}