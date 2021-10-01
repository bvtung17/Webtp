using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Webthucpham.Api;
using Webthucpham.Models;
using Webthucpham.ViewModels.Catalog.Products;

namespace Webthucpham.Controllers
{
    public class HomeController : ClientBaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISlideApiClient _slideApiClient;
        private readonly IProductApiClient _productApiClient;
        private readonly ICategoryApiClient _categoryApiClient;
       
        public HomeController(
            ILogger<HomeController> logger,
            ISlideApiClient slideApiClient,
            IProductApiClient productApiClient,
            ICategoryApiClient categoryApiClient, IClientApi clientApi) : base(clientApi)
        {
            _logger = logger;
            _slideApiClient = slideApiClient;
            _productApiClient = productApiClient;
            _categoryApiClient = categoryApiClient;
           
        }

        public async Task<IActionResult> Index()
        {
            var clientId = User.Claims.ToList().Where(x => x.Type == "Id").FirstOrDefault();
            var logout = HttpContext.Session.GetString("Token") == null && clientId != null;
            if (logout)
                return RedirectToAction("Logout", "User");
            await CreateUserViewBag();
            var slides = await _slideApiClient.GetAll();
            ////var banners = await _bannerApiClient.GetAll();
            var homeViewModel = new HomeViewModel()
            {
                Slides = slides,
            };
            CreateCartViewBag();
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return View(homeViewModel);
        }

        [HttpGet("Search/{categoryId}"), HttpGet("Search")]
        public async Task<IActionResult> Search(GetProductRequest request)
        {
            var clientId = User.Claims.ToList().Where(x => x.Type == "Id").FirstOrDefault();
            var logout = HttpContext.Session.GetString("Token") == null && clientId != null;
            if (logout)
                return RedirectToAction("Logout", "User");
            await CreateUserViewBag();

            var products = await _productApiClient.SearchProduct(request);

            ViewBag.request = request;
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return View(products);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
