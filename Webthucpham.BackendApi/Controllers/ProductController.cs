using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webthucpham.Application.Catalog.Products;

namespace Webthucpham.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IPublicProductService _pubicProductService;
        public ProductController(IPublicProductService pubicProductService)
        {
            _pubicProductService = pubicProductService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _pubicProductService.GetAll();


            return Ok(products);
        }
    }
}
