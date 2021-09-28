using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.Application.Catalog.Clients;
using Webthucpham.ViewModels.System.Clients;

namespace Webthucpham.WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(ClientLoginRequest request)
        {
            var result = await _clientService.Login(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(ClientRegisterRequest request)
        {
            var result = await _clientService.Register(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("{clientId}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(Guid clientId)
        {
            var result = await _clientService.GetDetail(clientId);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);

        }
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetClientById(Guid id)
        {
            var client = await _clientService.GetClientById(id);
            if (client == null)
            {
                return BadRequest(id);
            }
            return Ok(client);
        }
        [HttpGet("{id}/details/orders")]
        public async Task<IActionResult> GetOrderByClientId(Guid id)
        {
            var orders = await _clientService.GetOrderByClient(id);
            return Ok(orders);
        }

        [HttpPut("{clientId}")]
        [Authorize]
        public async Task<IActionResult> Update([FromForm] ClientUpdateViewModel request, Guid clientId)
        {

            var result = await _clientService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> Get([FromQuery] GetClientPagingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var clients = await _clientService.GetClientPaging(request);
            return Ok(clients);
        }
    }
}