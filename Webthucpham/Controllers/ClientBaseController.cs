using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Webthucpham.Api;
using Webthucpham.ViewModels.Catalog.Carts;

namespace Webthucpham.Controllers
{
    public class ClientBaseController : Controller
    {
        private readonly IClientApi _clientApi;

        public ClientBaseController(IClientApi clientApi)
        {
            _clientApi = clientApi;
        }
        protected async Task CreateUserViewBag()
        {
            var result = GetClaim("Id");
            ViewBag.IsLogin = result != null;
            if (result != null)
            {

                var clientId = new Guid(result.Value);
                var response = await _clientApi.GetDetail(clientId);
                var client = response.ResultObj;
                ViewBag.Avatar = !String.IsNullOrEmpty(client.Avatar) ? $"http://localhost:8000/user-content/{client.Avatar}" : "/images/default.jpg";
                ViewBag.Name = client.Name;

            }
        }
        protected void CreateCartViewBag()
        {
            var cartJs = HttpContext.Session.GetString("Cart");
            if (cartJs != null)
            {
                var cart = JsonConvert.DeserializeObject(cartJs);
                ViewBag.Cart = cart;
            }
        }

        protected ClientCartViewModel GetCartViewModel()
        {
            var cartJS = HttpContext.Session.GetString("Cart");
            if (cartJS == null) { return null; }
            var cart = JsonConvert.DeserializeObject<ClientCartViewModel>(cartJS);
            return cart;
        }
        protected Claim GetClaim(string type)
        {
            var userClaims = User.Claims.ToList();

            return userClaims.Where(x => x.Type == type).FirstOrDefault();

        }

    }
}
