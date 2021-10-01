using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Api;
using Webthucpham.ViewModels.Catalog.Carts;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.System.Clients;

namespace Webthucpham.Controllers
{
    public class UserController : ClientBaseController
    {
        private readonly IConfiguration _config;
        private readonly IClientApi _clientApi;
        private readonly ICartApiClient _cartApiClient;
        private readonly IClientOrderApi _clientOrderApi;
        private readonly IOrderApiClient _orderApiClient;
        private readonly ICategoryApiClient _categoryApiClient;

        public UserController(
            IClientApi clientApi, IConfiguration configuration
            , ICartApiClient cartApiClient,
            IClientOrderApi clientOrderApi,
            ICategoryApiClient categoryApiClient,
            IOrderApiClient orderApiClient
            ) : base(clientApi)
        {
            _clientApi = clientApi;
            _config = configuration;
            _cartApiClient = cartApiClient;
            _clientOrderApi = clientOrderApi;
            _orderApiClient = orderApiClient;
            _categoryApiClient = categoryApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> LoginAsync()
        {

            var sessions = HttpContext.Session.GetString("Token");
            if (sessions != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var cartJs = HttpContext.Session.GetString("Cart");
            if (cartJs != null)
            {
                var cart = JsonConvert.DeserializeObject<ClientCartViewModel>(cartJs);
                ViewBag.Cart = cart;
            }
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(ClientLoginRequest request)
        {
            CreateCartViewBag();
            await CreateUserViewBag();
            var sessions = HttpContext.Session.GetString("Token");
            if (sessions != null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            var apiResult = await _clientApi.Login(request);
            if (!apiResult.IsSuccessed)
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác";
                return View(request);
            }

            var userPrincipal = ValidateToken(apiResult.ResultObj);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                IsPersistent = true
            };

            var test = userPrincipal.Claims.ToList();

            HttpContext.Session.SetString("Token", apiResult.ResultObj);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                authProperties);
            var cartJs = HttpContext.Session.GetString("Cart");
            if (cartJs != null)
            {
                var clientId = test.Where(x => x.Type == "Id").FirstOrDefault().Value;
                var cart = JsonConvert.DeserializeObject<ClientCartViewModel>(cartJs);
                cart.ClientId = new Guid(clientId);
                cart = await _cartApiClient.AddToCart(cart);
                ViewBag.Cart = cart;
            }


            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {

            var cartJs = HttpContext.Session.GetString("Cart");
            if (cartJs != null)
            {
                var cart = JsonConvert.DeserializeObject<ClientCartViewModel>(cartJs);
                ViewBag.Cart = cart;
            }
            var sessions = HttpContext.Session.GetString("Token");
            if (sessions != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(ClientRegisterRequest request)
        {
            CreateCartViewBag();
            await CreateUserViewBag();
            if (!ModelState.IsValid)
                return View(request);

            var apiResult = await _clientApi.Register(request);
            if (!apiResult.IsSuccessed)
            {
                ViewBag.Error = apiResult.ResultObj ?? apiResult.Message;
                return View(request);
            }
            var userPrincipal = ValidateToken(apiResult.ResultObj);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                IsPersistent = true
            };

            HttpContext.Session.SetString("Token", apiResult.ResultObj);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                authProperties);
            CreateCartViewBag();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Detail()
        {
            var claims = User.Claims.ToList();
            var isLogin = claims.Where(x => x.Type == "Id").FirstOrDefault() != null;
            if (!isLogin)
            {
                return RedirectToAction("Error", "Home");
            }

            await CreateUserViewBag();
            CreateCartViewBag();
            var clientId = claims.Where(x => x.Type == "Id").FirstOrDefault().Value;
            var client = await GetClientViewModel(clientId);
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return View(client);
        }
        public async Task<ClientUpdateViewModel> GetClientViewModel(string clientId)
        {
            var response = await _clientApi.GetDetail(new Guid(clientId));
            var client = response.ResultObj;
            string avatar = "";
            var isDefaultAvatar = String.IsNullOrEmpty(client.Avatar);
            avatar = isDefaultAvatar ? "/images/default.jpg" : _config["BaseImageAddress"] + client.Avatar;
            client.Avatar = avatar;
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return client;
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Detail([FromForm] ClientUpdateViewModel request)
        {
            var clientId = User.Claims.ToList().Where(x => x.Type == "Id").FirstOrDefault().Value;
            var client = await GetClientViewModel(clientId);

            if (!ModelState.IsValid)
            {
                CreateCartViewBag();
                ViewBag.Error = "Cập nhật thông tin không thành công";
                return View(client);
            }

            request.Id = new Guid(clientId);
            var result = await _clientApi.Update(request);
            CreateCartViewBag();
            await CreateUserViewBag();
            if (!result.IsSuccessed)
            {
                ViewBag.Error = result.Message;
                ModelState.AddModelError("", result.Message);
                return View(client);
            }
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            client = await GetClientViewModel(clientId);
            ViewBag.Result = "Cập nhật thông tin thành công";
            return View(client);
        }
        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;

            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _config["Tokens:Issuer"];
            validationParameters.ValidIssuer = _config["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory([FromQuery] GetOrderRequest request, string status)
        {

            var token = HttpContext.Session.GetString("Token");
            var clientIdClaim = GetClaim("Id");
            if (token == null && clientIdClaim == null)
            {
                return RedirectToAction("Error", "Home");
            }
            await CreateUserViewBag();
            CreateCartViewBag();
            var testClientId = new Guid(clientIdClaim.Value);
            var orders = await _clientOrderApi.GetOrderHistory(testClientId, request, status);
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var token = HttpContext.Session.GetString("Token");
            var clientIdClaim = GetClaim("Id");
            if (token == null && clientIdClaim == null)
            {
                return RedirectToAction("Error", "Home");
            }
            await CreateUserViewBag();
            CreateCartViewBag();
            var testClientId = new Guid(clientIdClaim.Value);
            var order = await _clientOrderApi.GetClientOrderDetails(testClientId, id);
            var products = await _orderApiClient.GetProducts(id);
            order.OrderProducts = products;
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return View(order);
        }
        [HttpPut]
        public async Task<JsonResult> cancelOrder(int orderId, string cancelReason)
        {

            var result = await _clientOrderApi.ClientCancelOrder(orderId, cancelReason);

            return new JsonResult(new { message = result.Message, isSuccess = result.IsSuccessed });
        }
    }
}

