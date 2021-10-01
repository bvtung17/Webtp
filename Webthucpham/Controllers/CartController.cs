
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Carts;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.Api;
using Webthucpham.Controllers;

namespace CosmeticsShop.WebApp.Controllers
{
    public class CartController : ClientBaseController
    {
        private readonly ICartApiClient _cartApiClient;
        private readonly IClientOrderApi _clientOrderApi;
        private readonly IClientApi _clientApi;
        private readonly ICategoryApiClient _categoryApiClient;

        public CartController(ICartApiClient cartApiClient, IClientOrderApi clientOrderApi, ICategoryApiClient categoryApiClient, IClientApi clientApi) : base(clientApi)
        {
            _cartApiClient = cartApiClient;
            _clientOrderApi = clientOrderApi;
            _categoryApiClient = categoryApiClient;
            _clientApi = clientApi;
        }

        [HttpGet]
        public async Task<IActionResult> CartDetail()
        {
            await CreateUserViewBag();
            var cartJS = HttpContext.Session.GetString("Cart");
            if (cartJS != null)
            {
                var cart = JsonConvert.DeserializeObject<ClientCartViewModel>(cartJS);
                ViewBag.Cart = cart;
            }
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> InforOrder()
        {
            await CreateUserViewBag();
            var isLogin = HttpContext.Session.GetString("Token");
            Claim clientIdClaim = null;
            ViewBag.IsLogin = false;
            if (isLogin != null)
            {
                ViewBag.IsLogin = true;
                clientIdClaim = GetClaim("Id");
            }
            var cartJS = HttpContext.Session.GetString("Cart");
            if (cartJS == null)
            {
                return RedirectToAction("CartDetail", "Cart");
            }

            var cart = JsonConvert.DeserializeObject<ClientCartViewModel>(cartJS);
            ViewBag.Cart = cart;
            CreateCartViewBag();
            if (isLogin != null)
            {
                var ClientId = new Guid(clientIdClaim.Value);
                var client = await _clientApi.GetDetail(ClientId);
                ViewBag.User = client.ResultObj;
            }
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return View();
        }

        [HttpGet("{cartId}/thanks/{orderId}")]
        public async Task<IActionResult> Bill(Guid cartId, int orderId)
        {
            await CreateUserViewBag();
            var cart = GetCartViewModel();

            if (cart != null)
            {
                HttpContext.Session.Remove("Cart");
            }

            if (orderId < 1)
            {
                return RedirectToAction("Error", "Home");
            }

            var order = await _clientOrderApi.GetOrder(cartId, orderId);

            if (!order.IsSuccessed)
            {
                return RedirectToAction("Error", "Home");
            }
            var clientCart = await _cartApiClient.GetCart(cartId);
            ViewBag.Order = order.ResultObj;
            ViewBag.Cart = clientCart.ResultObj;
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InforOrder(ClientCreateOrderViewModel request)
        {

            var cart = GetCartViewModel();
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            ViewBag.Cart = cart;
            request.ClientCart = cart;
            var isLogin = HttpContext.Session.GetString("Token");
            ViewBag.isLogin = false;
            var clientId = GetClaim("Id")?.Value ?? null;

            if (isLogin != null && clientId != null)
            {
                request.ClientID = new Guid(clientId);
                ViewBag.isLogin = true;
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result = await _clientOrderApi.ClientCreateOrder(request);
            if (result < 1)
            {
                return View();
            }
            ViewBag.Categories = productCategory;
            return RedirectToAction("bill", new { orderId = result, cartId = cart.Id });
        }
        [HttpPost]
        public async Task<JsonResult> AddToCart([FromBody] ProductInCartViewModel productCart)
        {
            var cartJS = HttpContext.Session.GetString("Cart");
            ClientCartViewModel cart;
            var token = HttpContext.Session.GetString("Token");

            if (cartJS != null)
            {
                cart = JsonConvert.DeserializeObject<ClientCartViewModel>(cartJS);
                var product = cart.Products.Where(x => x.Id == productCart.Id).FirstOrDefault();
                if (product != null)
                {
                    product.Quantity += productCart.Quantity;
                }
                else
                {
                    cart.Products.Add(productCart);
                }

                if (token != null)
                {
                    var clientId = User.Claims.ToList().Where(x => x.Type == "Id").FirstOrDefault().Value;
                    cart.ClientId = new Guid(clientId);
                }

                cart.CartPrice = cart.Products.Sum(x => x.ProductPrice * x.Quantity);
                cart = await _cartApiClient.AddToCart(cart);
                cartJS = JsonConvert.SerializeObject(cart);
                HttpContext.Session.SetString("Cart", cartJS);
            }
            else
            {
                cart = new ClientCartViewModel();
                cart.Products.Add(productCart);
                cart.CartPrice = productCart.ProductPrice;
                if (token != null)
                {
                    var clientId = User.Claims.ToList().Where(x => x.Type == "Id").FirstOrDefault().Value;
                    cart.ClientId = new Guid(clientId);

                }
                cart = await _cartApiClient.AddToCart(cart);
                var serializeCart = JsonConvert.SerializeObject(cart);
                HttpContext.Session.SetString("Cart", serializeCart);
            }
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return new JsonResult(cart);
        }
        [HttpDelete]
        public async Task<JsonResult> RemoveProduct(int removeId)
        {
            var cart = GetCartViewModel();
            var isExistInCart = cart.Products.Where(x => x.Id == removeId).FirstOrDefault() != null;
            if (removeId < 1 || !isExistInCart)
            {
                return new JsonResult(new { result = false, message = "Sản phẩm không tồn tại" });

            }

            var request = new DeleteProductInCartRequest() { Cart = cart, ProductId = removeId };
            var response = await _cartApiClient.RemoveProduct(request);
            if (!response.IsSuccessed)
            {
                return new JsonResult(new { result = false, message = "Có lỗi trong quá trình xóa" });
            }
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            cart = response.ResultObj;
            var cartJs = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Cart", cartJs);
            return new JsonResult(new { result = true, message = "Xóa sản phẩm khỏi cart thành công" });
        }

        [HttpPut]
        public async Task<JsonResult> UpdateCart(int productId, bool increment)
        {
            var cart = GetCartViewModel();
            var product = cart.Products.Where(x => x.Id == productId).FirstOrDefault();

            if (product == null)
            {
                return new JsonResult(new { status = 400, message = "Không tìm thấy sản phẩm" });
            }
            if (increment)
            {
                product.Quantity += 1;
            }
            else
            {
                product.Quantity -= 1;
            }
            var isRemove = !increment && product.Quantity == 0;
            if (isRemove)
            {
                cart.Products.Remove(product);
            }
            cart.CartPrice = cart.Products.Sum(x => x.ProductPrice * x.Quantity);

            cart = await _cartApiClient.AddToCart(cart);
            var serializeCart = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Cart", serializeCart);
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return new JsonResult(new { status = 201, message = "Ok", hasRemove = isRemove, newQuantity = product.Quantity, newTotalPrice = product.Quantity * product.ProductPrice, newCartPrice = cart.CartPrice });
        }
    }
}
