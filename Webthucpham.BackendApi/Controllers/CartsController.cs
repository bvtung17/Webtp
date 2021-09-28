using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.Application.Catalog.Carts;
using Webthucpham.ViewModels.Catalog.Carts;

namespace Webthucpham.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : Controller
    {
        private readonly ICartService _cartService;
        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(ClientCartViewModel request)
        {
            var result = await _cartService.AddToCart(request);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientCart(Guid id)
        {
            var result = await _cartService.GetClientCart(id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> RemoveProductInCart(DeleteProductInCartRequest request)
        {
            var result = await _cartService.RemoveProductInCart(request);
            return Ok(result);
        }
    }
}