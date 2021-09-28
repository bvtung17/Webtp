using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webthucpham.Application.Catalog.Orders;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.Sales;

namespace Webthucpham.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        private IOrderService _orderService;
        public OrdersController(IOrderService orderSerive)
        {
            _orderService = orderSerive;
        }
        [HttpGet("paging")]
        [Authorize]
        public async Task<IActionResult> Index([FromQuery] GetOrderRequest request, string status)
        {
            if (!ModelState.IsValid || request.PageSize <= 0)
            {
                return Ok(new PageResponse<OrderViewModel>() { Items = new List<OrderViewModel>(), PageIndex = 1, PageSize = 5, TotalRecords = 0 });
            }

            var orders = await _orderService.GetAll(request, status);
            return Ok(orders);
        }
        [HttpGet("client/{clientId}/paging")]
        public async Task<IActionResult> ClientGetOrderHistory([FromQuery] GetOrderRequest request, string status, Guid clientId)
        {
            if (!ModelState.IsValid || request.PageSize <= 0)
            {
                return Ok(new PageResponse<ClientOrderHistoryViewMode>() { Items = new List<ClientOrderHistoryViewMode>(), PageIndex = 1, PageSize = 5, TotalRecords = 0 });
            }
            var orders = await _orderService.ClientGetOrderHistory(clientId, request, status);
            return Ok(orders);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id == 0)
            {
                return BadRequest(id);
            }

            var order = await _orderService.GetById(id);
            if (order == null)
            {
                return BadRequest(id);
            }
            return Ok(order);
        }

        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetOrderProducts(int id)
        {
            if (id <= 0)
            {
                return BadRequest(id);
            }
            var products = await _orderService.GetOrderProducts(id);
            return Ok(products);
        }

        [HttpGet("{clientId}/order/{orderId}")]
        public async Task<IActionResult> GetClientOrderDetails(int orderId, Guid clientId)
        {
            if (orderId == 0)
            {
                return BadRequest(orderId);
            }
            var order = await _orderService.GetclientOrderDetails(clientId, orderId);
            if (order == null)
            {
                return BadRequest("Không tìm thấy đơn hàng!");
            }
            return Ok(order);
        }
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus(OrderViewModel request)
        {
            var result = await _orderService.UpdateStatus(request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPost("client")]
        public async Task<IActionResult> ClientCreateOrder(ClientCreateOrderViewModel request)
        {
            var result = await _orderService.ClientCreateOrder(request);
            return Ok(result);
        }
        [HttpGet("client/{cartId}/order/{orderId}")]
        public async Task<IActionResult> ClientGetOrder(Guid cartId, int orderId)
        {
            var result = await _orderService.ClientGetOrder(cartId, orderId);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> ClientCancelOrder(int orderId, [FromBody] ClientCancelOrderReasonRequest request)
        {
            var result = await _orderService.ClientCancelOrder(orderId, request.Message);

            return Ok(result);
        }
    }
}
