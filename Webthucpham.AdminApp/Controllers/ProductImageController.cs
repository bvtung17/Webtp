using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.Api;
using Webthucpham.ViewModels.Catalog.ProductImages;

namespace Webthucpham.AdminApp.Controllers
{
    public class ProductImageController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        public ProductImageController(IProductApiClient productApiClient)
        {
            _productApiClient = productApiClient;
        }

        [HttpGet("product/images/{imageId}/edit")]
        public async Task<IActionResult> Edit(int imageId, [FromQuery] int productId)
        {

            if (!ModelState.IsValid || imageId == 0 || productId == 0)
            {
                return RedirectToAction("Error", "Home");
            }
            var image = await _productApiClient.GetImageById(productId, imageId);
            if (image == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var model = new ProductImageUpdateRequest()
            {
                Id = imageId,
                ProductId = productId,
                Caption = image.Caption
            };
            return View(model);
        }

        [HttpPost("product/images/{imageId}/edit")]
        public async Task<IActionResult> Edit(ProductImageUpdateRequest request)
        {

            if (!ModelState.IsValid || request.Id == 0)
            {
                return View(request);
            }
            var result = await _productApiClient.UpdateProductImage(request);
            if (!result)
                return View(request);
            TempData["result"] = "Sửa ảnh thành công!";
            return LocalRedirect($"/Product/{request.ProductId}/images");
        }
        [HttpDelete("product/{productId}/images")]
        public async Task Delete(int productId, [FromQuery] int imageId)
        {

            if (!ModelState.IsValid || productId < 1 || imageId < 1)
            {

                LocalRedirect($"/Product/{productId}/images");
            }
            var result = await _productApiClient.DeleteImage(productId, imageId);
            if (!result.IsSuccessed || !result.ResultObj)
            {
                TempData["error"] = result.Message;
                LocalRedirect($"/Product/{productId}/images");
            }
            else
            {
                TempData["result"] = result.Message;
                LocalRedirect($"/Product/{productId}/images");
            }
        }

        [HttpGet("product/{productId}/images/add")]
        public IActionResult Add(int productId)
        {

            if (!ModelState.IsValid || productId == 0)
            {
                return RedirectToAction("Error", "Home");
            }

            var model = new ProductImageCreateRequest()
            {
                ProductId = productId
            };
            return View(model);
        }

        [HttpPost("product/{productId}/images/add")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Add(ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }


            var result = await _productApiClient.AddImage(request);

            if (!result)
            {
                return View(request);
            }

            TempData["result"] = "Thêm ảnh thành công!";
            return LocalRedirect($"/Product/{request.ProductId}/images");
        }

    }
}
