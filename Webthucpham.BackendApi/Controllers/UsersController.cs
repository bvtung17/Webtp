using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webthucpham.Application.System.Users;
using Webthucpham.ViewModels.System.Users;

namespace Webthucpham.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly IUserService _userservice;
        public UsersController(IUserService userService)
        {
            _userservice = userService;
        }


        // ĐĂNG NHẬP
        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultToken = await _userservice.Authencate(request);
            if (string.IsNullOrEmpty(resultToken))
            {
                return BadRequest("Tài khoản hoặc mật khẩu không đúng");
            }
            return Ok(resultToken);
        }


        // ĐĂNG KÝ
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userservice.Register(request);
            if (!result)
            {
                return BadRequest("Không hỗ trợ đăng ký");
            }
            return Ok();
        }

    }
}
