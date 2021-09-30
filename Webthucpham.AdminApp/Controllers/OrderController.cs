using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.Api;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.AdminApp.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderApiClient _orderApiClient;
        public OrderController(IOrderApiClient orderApiClient)
        {
            _orderApiClient = orderApiClient;
        }
        public async Task<IActionResult> Index([FromQuery] GetOrderRequest request, string status)
        {
            var orders = await _orderApiClient.GetAll(request, status);
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            var categories = OrderCategorySearch.Categories;
            var category = categories.Where(x => x.Value == request.Type).FirstOrDefault();
            foreach (var c in categories)
            {
                if (c.Value != request.Type)
                {
                    c.Selected = false;
                    continue;
                }
                c.Selected = true;
            }

            ViewBag.Categories = categories;
            return View(orders);
        }
        [HttpGet("order/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var order = await _orderApiClient.GetById(id);
            var products = await _orderApiClient.GetProducts(id);
            order.OrderProducts = products;
            return View(order);
        }
        [HttpPost("order/{id}")]
        public async Task<IActionResult> Update(OrderViewModel request)
        {
            if (request.Id <= 0)
            {
                ViewBag.Error = "Cập nhập không thành công!";
                return View(request);
            }
            var result = await _orderApiClient.UpdateStatus(request);
            if (!result)
            {
                ViewBag.Error = "Cập nhật không thành công!";
                return View(request);
            }
            TempData["result"] = "Cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction("Index");

        }
    }
}
