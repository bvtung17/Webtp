using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.System.Users
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Yêu Cầu Nhập Tên Đăng Nhập");
            RuleFor(x=>x.Password).NotEmpty().WithMessage("Yêu Cầu Nhập Nhập Mật Khẩu")
                .MinimumLength(6).WithMessage("Mật khẩu không được ít hơn 6 ký tự");


        }
    }
}
