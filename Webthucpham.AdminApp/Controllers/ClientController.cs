using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.Api;
using Webthucpham.ViewModels.System.Clients;

namespace Webthucpham.AdminApp.Controllers
{
    public class ClientController : BaseController
    {
        private readonly IClientApi _clientApi;
        private readonly IClientOrderApi _clientOrderApi;
        private readonly IConfiguration _config;

        public ClientController(IClientApi clientApi, IClientOrderApi clientOrderApi,
            IConfiguration config
         )
        {
            _clientApi = clientApi;
            _clientOrderApi = clientOrderApi;
            _config = config;
        }
        public async Task<IActionResult> Index(string keyword = "", int pageIndex = 1, int pageSize = 10)
        {

            var request = new GetClientPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };

            var data = await _clientApi.GetClientPaging(request);
            ViewBag.Keyword = keyword;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.ResultObj);
        }

        [HttpGet("client/{id}/details")]
        public async Task<IActionResult> Details(string id)
        {
            var client = await _clientApi.GetByClientId(new Guid(id));
            var order = await _clientApi.GetOrderByClientId(new Guid(id));
            client.Orders = order;
            return View(client);
        }


        [HttpPut]
        public async Task<JsonResult> cancelOrder(int orderId, string cancelReason)
        {

            var result = await _clientOrderApi.ClientCancelOrder(orderId, cancelReason);

            return new JsonResult(new { message = result.Message, isSuccess = result.IsSuccessed });
        }
    }
}
