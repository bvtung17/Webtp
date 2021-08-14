using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webthucpham.Application.Catalog.Products;
using Webthucpham.ViewModels.Catalog.Products;

namespace Webthucpham.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IPublicProductService _pubicProductService;
        private readonly IManageProductService _manageProductService;
        public ProductController(IPublicProductService pubicProductService, IManageProductService manageProductService)
        {
            _pubicProductService = pubicProductService;

            _manageProductService = manageProductService;
        }
        //http://locahost:port/product
        [HttpGet("languageId")]
        public async Task<IActionResult> Get(string languageId)
        {
            var products = await _pubicProductService.GetAll(languageId);


            return Ok(products);
        }
        //http://locahost:port/product/public-paging
        [HttpGet("public-paging/{languageId}")]
        public async Task<IActionResult> Get([FromQuery] GetPublicProductPagingRequest request) // lay tu query
        {
            var products = await _pubicProductService.GetAllByCategoryId(request);
            return Ok(products);
        }


        //GetByid

        //http://locahost:port/product/id
        [HttpGet("{id}/{languageId}")]
        public async Task<IActionResult> GetById(int id , string languageId)
        {
            var product = await _manageProductService.GetById(id, languageId);
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
            var productId = await _manageProductService.Create(request);
            if (productId == 0)
            {
                return BadRequest();
            }
            var product = await _manageProductService.GetById(productId,request.LanguageId);
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var affecterResult = await _manageProductService.Delete(id);
            if (affecterResult == 0)
            {
                return BadRequest();
            }
            return Ok();
        }
        //phuong thuc UpDATE PRICE
        [HttpPut("price/{id}/{newPrice}")]
        public async Task<IActionResult> UpdatePrice(int id, decimal newPrice)
        {
            var isSuccesfull = await _manageProductService.UpdatePrice(id,newPrice);
            if (isSuccesfull)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}