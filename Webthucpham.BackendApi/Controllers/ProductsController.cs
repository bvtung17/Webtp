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
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.Catalog.Products.Manage;

namespace Webthucpham.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
   
        private readonly IProductService _productServices;
        public ProductsController(IProductService productService)
        {

            _productServices = productService;
        }

        //http://locahost:port/product/PageIndex=1
        [HttpGet("paging")]
        public async Task<IActionResult> Get([FromQuery] GetProductRequest request, string status)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var products = await _productServices.GetAll(request, status);
            return Ok(products);
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedProducts()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var products = await _productServices.GetFeaturedProducts();

            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProductClient([FromQuery] GetProductRequest request)
        {
            var categories = await _productServices.SearchProductClient(request);
            return Ok(categories);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var product = await _productServices.GetById(id);
            if (product == null)
            {
                return BadRequest($"Cannot find product with id: {id}");
            }

            return Ok(product);
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var productId = await _productServices.Create(request);
            if (productId == 0)
            {
                return BadRequest();
            }

            var product = await _productServices.GetById(productId);
            product.Id = productId;

            return CreatedAtAction("Create", product);
        }
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var affectedResult = await _productServices.Update(request);
            if (affectedResult == 0)
            {
                return BadRequest();
            }

            return Ok("Updated");
        }

        [HttpPut("{id}/categories")]
        [Authorize]

        public async Task<IActionResult> CategoryAssign([FromBody] CategoryAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _productServices.CategoryAssign(request);
            if (!response.IsSuccessed)
            {
                return BadRequest("Assign category is not ok!");
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize]

        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var deleted = await _productServices.Delete(id);
            if (deleted == null)
            {
                return BadRequest($"Không tồn tại id: {id}");
            }

            return Ok("Deleted");
        }

        [HttpPatch("{id}/price")]
        [Authorize]

        public async Task<IActionResult> UpdatePrice([FromBody] decimal newPrice, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _productServices.UpdatePrice(id, newPrice) ?? false;
            if (!result)
            {
                return BadRequest();
            }

            return Ok("Updated");
        }

        // Product Images

        [HttpGet("{productId}/images/{id}")]
        public async Task<IActionResult> GetByImageId(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var image = await _productServices.GetImageById(id);
            if (image == null)
            {
                return BadRequest($"Không tồn tại sản phẩm có id: {id}");
            }

            return Ok(image);
        }

        [HttpGet("images/{id}")]

        public async Task<IActionResult> GetAllPaging(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var images = await _productServices.GetImageById(id);
            if (images == null)
            {
                return BadRequest($"Không tồn tại sản phẩm có id: {id}");
            }

            return Ok(images);
        }

        [HttpGet("{productId}/images")]
        [Authorize]
        public async Task<IActionResult> GetProductImages(int productId, [FromQuery] PagingRequestBase request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var images = await _productServices.GetImages(productId, request);
            if (images == null)
            {
                return BadRequest($"Không tồn tại sản phẩm có id: {productId}");
            }

            return Ok(images);
        }

        [HttpPut("{productId}/images/{imageId}/thumbnail")]
        [Authorize]
        public async Task<IActionResult> ChangeThumbnail(int productId, int imageId)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var affectedResult = await _productServices.ChangeThumbnail(productId, imageId);
            if (!affectedResult)
            {
                return BadRequest();
            }

            return Ok("Updated");
        }

        [HttpPut("images/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateImage(int id, [FromForm] ProductImageUpdateRequest request)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var affectedResult = await _productServices.UpdateImage(id, request);
            if (affectedResult == 0)
            {
                return BadRequest();
            }

            return Ok("Updated");
        }

        [HttpPost("{id}/images")]
        [Consumes("multipart/form-data")]
        [Authorize]

        public async Task<IActionResult> AddImage(int id, [FromForm] ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var imageId = await _productServices.AddImage(id, request);
            if (imageId < 1)
            {
                return BadRequest();
            }

            var image = await _productServices.GetImageById(imageId);

            return CreatedAtAction(nameof(GetByImageId), new { Id = imageId }, image);
        }

        [HttpDelete("{productId}/images/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteImage(int productId, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _productServices.RemoveImage(productId, id);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}/client")]
        public async Task<IActionResult> GetClientProductDetail(int id)
        {
            var result = await _productServices.ClientGetProductDetail(id);

            return Ok(result);

        }
    }
}