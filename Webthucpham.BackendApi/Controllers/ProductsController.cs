using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Webthucpham.Application.Catalog.Products;
using Webthucpham.ViewModels.Catalog.ProductImages;
using Webthucpham.ViewModels.Catalog.Products;

namespace Webthucpham.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
   
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {

            _productService = productService;
        }

        //http://locahost:port/product/PageIndex=1
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetManageProductPagingRequest request) // lay tu query
        {
            var products = await _productService.GetAllPaging(request);
            return Ok(products);
        }


        //GetByid

        //http://locahost:port/product/id
        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById(int productId, string languageId)
        {
            var product = await _productService.GetById(productId, languageId);
            if (product == null)
            {
                return BadRequest("Không tìm thấy sản phẩm");
            }
            return Ok(product);
        }

        //phuong thuc create product
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productId = await _productService.Create(request);
            if (productId == 0)
            {
                return BadRequest();
            }
            var product = await _productService.GetById(productId, request.LanguageId);
            return CreatedAtAction(nameof(GetById), new { id = productId }, product);
        }

        //phuong thuc UpDATE product
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            var affecterResult = await _productService.Update(request);
            if (affecterResult == 0)
            {
                return BadRequest();
            }
            return Ok();
        }

        //phuong thuc Delete product
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var affecterResult = await _productService.Delete(productId);
            if (affecterResult == 0)
            {
                return BadRequest();
            }
            return Ok();
        }
        //phuong thuc UpDATE PRICE
        [HttpPatch("{productId}/{newPrice}")]
        public async Task<IActionResult> UpdatePrice(int productId, decimal newPrice)
        {
            var isSuccesfull = await _productService.UpdatePrice(productId, newPrice);
            if (isSuccesfull)
            {
                return Ok();
            }
            return BadRequest();
        }

        //IMAGE

        //phuong thuc create IMAGE
        [HttpPost("{productId}/image)")]
        public async Task<IActionResult> CreateImage(int productId,  [FromForm] ProductImageCreateRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var imageId = await _productService.AddImage(productId, request);
            if (imageId == 0)
            {
                return BadRequest();
            }
            var image = await _productService.GetImageById(imageId);
            return CreatedAtAction(nameof(GetImageById), new { id = imageId }, image);
        }

        // UPDATE IMAGE
        [HttpPut("{productId}/image/{imageId})")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductUpdateImageRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.UpdateImage(imageId, request);
            if (result == 0)
            {
                return BadRequest();
            }
            var image = await _productService.GetImageById(imageId);
            return Ok();
        }

        // DELETE IMAGE
        [HttpDelete("{productId}/image/{imageId})")]
        public async Task<IActionResult> RemoveImage(int imageId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.RemoveImage(imageId);
            if (result == 0)
            {
                return BadRequest();
            }
            var image = await _productService.GetImageById(imageId);
            return Ok();
        }


        //GetByid ảnh

        //http://locahost:port/product/id
        [HttpGet("{productId}/image/{imageId}")]
        public async Task<IActionResult> GetImageById(int productId, int imageId)
        {
            var image = await _productService.GetImageById( imageId);
            if (image == null)
            {
                return BadRequest("Không tìm thấy sản phẩm");
            }
            return Ok(image);
        }
        //CATEGORY
        [HttpPut("{id}/categories")]
        public async Task<IActionResult> CategoryAssign(int id, [FromBody] CategoryAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.CategoryAssign(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}