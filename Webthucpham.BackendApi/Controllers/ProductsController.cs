using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webthucpham.Application.Catalog.Products;
using Webthucpham.ViewModels.Catalog.ProductImages;

namespace Webthucpham.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IPublicProductService _pubicProductService;
        private readonly IManageProductService _manageProductService;
        public ProductsController(IPublicProductService pubicProductService, IManageProductService manageProductService)
        {
            _pubicProductService = pubicProductService;

            _manageProductService = manageProductService;
        }

        //http://locahost:port/product/PageIndex=1&pagesize=10&CategoryId=
        [HttpGet("public-paging/{languageId}")]
        public async Task<IActionResult> GetAllPaging(string languageId, [FromQuery] GetPublicProductPagingRequest request) // lay tu query
        {
            var products = await _pubicProductService.GetAllByCategoryId(languageId, request);
            return Ok(products);
        }


        //GetByid

        //http://locahost:port/product/id
        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById(int productId, string languageId)
        {
            var product = await _manageProductService.GetById(productId, languageId);
            if (product == null)
            {
                return BadRequest("Không tìm thấy sản phẩm");
            }
            return Ok(product);
        }

        //phuong thuc create product
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productId = await _manageProductService.Create(request);
            if (productId == 0)
            {
                return BadRequest();
            }
            var product = await _manageProductService.GetById(productId, request.LanguageId);
            return CreatedAtAction(nameof(GetById), new { id = productId }, product);
        }

        //phuong thuc UpDATE product
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            var affecterResult = await _manageProductService.Update(request);
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
            var affecterResult = await _manageProductService.Delete(productId);
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
            var isSuccesfull = await _manageProductService.UpdatePrice(productId, newPrice);
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
            var imageId = await _manageProductService.AddImage(productId, request);
            if (imageId == 0)
            {
                return BadRequest();
            }
            var image = await _manageProductService.GetImageById(imageId);
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
            var result = await _manageProductService.UpdateImage(imageId, request);
            if (result == 0)
            {
                return BadRequest();
            }
            var image = await _manageProductService.GetImageById(imageId);
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
            var result = await _manageProductService.RemoveImage(imageId);
            if (result == 0)
            {
                return BadRequest();
            }
            var image = await _manageProductService.GetImageById(imageId);
            return Ok();
        }


        //GetByid ảnh

        //http://locahost:port/product/id
        [HttpGet("{productId}/image/{imageId}")]
        public async Task<IActionResult> GetImageById(int productId, int imageId)
        {
            var image = await _manageProductService.GetImageById( imageId);
            if (image == null)
            {
                return BadRequest("Không tìm thấy sản phẩm");
            }
            return Ok(image);
        }

    }
}