using Webthucpham.ViewModels.System.Users;
using Webthucpham.Application.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmetics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Authenticate(request);
            if (string.IsNullOrEmpty(result.ResultObj))
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var registerResult = await _userService.Register(request);

            if (!registerResult.IsSuccessed)
            {
                return BadRequest(registerResult);
            }

            return Ok(registerResult);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserViewModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var registerResult = await _userService.Update(id, request);

            if (!registerResult.IsSuccessed)
            {
                return BadRequest("Update is not success!");
            }

            return Ok(registerResult);
        }

        [HttpPut("{id}/roles")]
        public async Task<IActionResult> RoleAssign([FromBody] RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.RoleAssign(request);

            if (!result.IsSuccessed)
            {
                return BadRequest(" Assign role is not ok!");
            }

            return Ok(result);
        }


        [HttpGet("paging")]
        public async Task<IActionResult> Get([FromQuery] GetUserPagingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var users = await _userService.GetUserPaging(request);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.GetById(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Delete(id);
            return Ok(result);
        }

    }
}
