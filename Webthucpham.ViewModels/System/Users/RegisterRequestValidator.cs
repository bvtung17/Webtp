using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.System.Users
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Yêu Cầu Nhập Tên ")
                .MaximumLength(200).WithMessage("Tền không vượt quá 200 ký tự");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Yêu Cầu Nhập Họ")
                .MaximumLength(200).WithMessage("Họ không vượt quá 200 ký tự");
            RuleFor(x => x.Dob).GreaterThan(DateTime.Now.AddYears(-200)).WithMessage("Tuổi không lớn hơn 200 năm");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Yêu Cầu Nhập Email").Matches(@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$")
                .WithMessage("Email không đúng định dạng");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Yêu Cầu Nhập Số Điện Thoại ");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Yêu Cầu Nhập Tên Đăng Nhập");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Yêu Cầu Nhập Nhập Mật Khẩu")
                .MinimumLength(6).WithMessage("Mật khẩu không được ít hơn 6 ký tự");

            RuleFor(x => x).Custom((request, context)
                  =>
              {
                  if (request.ConfirmPassword != request.Password)
                  {
                      context.AddFailure("Mật khẩu không trùng nhau");
                  }
              }
            );

        }
    }
}
