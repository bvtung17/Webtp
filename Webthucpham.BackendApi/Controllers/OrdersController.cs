using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webthucpham.Application.Catalog.Orders;
using Webthucpham.ViewModels.Sales;

namespace Webthucpham.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging([FromQuery] OrderPagingRequest request)
        {
            var order = await _orderService.GetAllPaging(request);
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetChart([FromQuery] OrderPagingRequest request)
        {
            var order = await _orderService.GetChart(request);
            return Ok(order);
        }

        [HttpGet]
        public IActionResult GetById([FromQuery] OrderViewModel request)
        {
            var order = _orderService.GetById(request);
            if (order == null)
                return BadRequest("Không tìm thấy order");
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var orderId = await _orderService.Create(request);
            if (orderId == 0)
                return BadRequest();

            return Ok("Tạo order thành công");
        }

        // PUT api/<ProductController>/5
        [HttpPut]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus([FromForm] int orderId, [FromForm] int status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var affectrs = await _orderService.UpdateStatus(orderId, status);
            if (!affectrs)
                return BadRequest();
            return Ok("Cập nhật trạng thái thành công");
        }

        // DELETE api/<ProductController>/5
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            var affectrs = await _orderService.Delete(id);
            if (affectrs == 0)
                return BadRequest();
            return Ok();
        }
    }
}
