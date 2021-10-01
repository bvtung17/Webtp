using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Webthucpham.ViewModels.System.Clients
{
    public class ClientUpdateValidator : AbstractValidator<ClientUpdateViewModel>
    {
        public ClientUpdateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên không để trống")
                .NotNull().WithMessage("Tên không thể để trống")
                .MinimumLength(3).WithMessage("Tên phải có ít nhất 3 kí tự");
            RuleFor(x => x.Address)
                .NotNull().WithMessage("Địa chỉ không để trống")
                .NotEmpty().WithMessage("Địa chỉ không để trống");
            RuleFor(x => x.NewPassword)
               .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
               .MaximumLength(21).WithMessage("Mật khẩu không thể quá 21 ký tự")
               .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,21}").WithMessage("Mật khẩu phải có ít nhất 1 ký tự đặc biệt, 1 chữ hoa ,1 chữ số và chữ thường!");
            RuleFor(x => x.RepeatPassword)

              .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
              .MaximumLength(21).WithMessage("Mật khẩu không thể quá 21 ký tự")
              .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,21}").WithMessage("Mật khẩu phải có ít nhất 1 ký tự đặc biệt, 1 chữ hoa ,1 chữ số và chữ thường!");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không thể để trống");
            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.NewPassword != request.RepeatPassword)
                {
                    context.AddFailure("Mật khẩu nhập lại không chính xác !");
                }
                var phoneRegex = @"(84|0[3|5|7|8|9])+([0-9]{8})\b";
                var reg = new Regex(phoneRegex);
                if (!reg.IsMatch(request?.PhoneNumber ?? ""))
                {
                    context.AddFailure("Số điện thoại không đúng định dạng");
                }
            });
        }
    }
}
