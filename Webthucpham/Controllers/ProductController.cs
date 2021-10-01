using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webthucpham.Api;
using Webthucpham.Models;
using Webthucpham.ViewModels.Catalog.Products;

namespace Webthucpham.Controllers
{
    public class ProductController : ClientBaseController
    {
        private readonly IClientApiProduct _clientApiProduct;
        private readonly ICategoryApiClient _categoryApiClient;
        private readonly IClientApi _clientApi;
        public ProductController(IClientApiProduct clientApiProduct, ICategoryApiClient categoryApiClient, IClientApi clientApi) : base(clientApi)
        {
            _clientApiProduct = clientApiProduct;
            _categoryApiClient = categoryApiClient;
            _clientApi = clientApi;
        }

        [HttpGet("product/{id}")]

        public async Task<IActionResult> Detail([FromRoute] int id)
        {
            if (id < 1)
            {
                return RedirectToAction("Error", "Home");
            }
            var response = await _clientApiProduct.GetProuctDetail(id);

            if (!response.IsSuccessed)
            {
                return RedirectToAction("Error", "Home");
            }
            await CreateUserViewBag();
            CreateCartViewBag();
            var productCategory = await _categoryApiClient.GetHomeProductCategories();
            ViewBag.Categories = productCategory;
            return View(response.ResultObj);
        }


    }
}
