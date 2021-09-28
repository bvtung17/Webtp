using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Webthucpham.ViewModels.System.Clients
{
    public class ClientRegisterValidation : AbstractValidator<ClientRegisterRequest>
    {
        public ClientRegisterValidation()
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Tên không được để trống");

            RuleFor(x => x.Address).NotEmpty().WithMessage("Địa chỉ không được để trống");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email không được để trống")
               .MaximumLength(100).WithMessage("Email không thể quá 100 ký tự")
               .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Không đúng định dạng email!");

            RuleFor(x => x.Password).NotEmpty()
               .WithMessage("Mật khẩu không được để trống")
               .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
               .MaximumLength(21).WithMessage("Mật khẩu không thể quá 21 ký tự")
               .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,21}").WithMessage("Mật khẩu phải có ít nhất 1 ký tự đặc biệt, 1 chữ hoa ,1 chữ số và chữ thường!");

            RuleFor(x => x.RepeatPassword).NotEmpty()
              .WithMessage("Mật khẩu không được để trống")
              .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
              .MaximumLength(21).WithMessage("Mật khẩu không thể quá 21 ký tự")
              .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,21}").WithMessage("Mật khẩu phải có ít nhất 1 ký tự đặc biệt, 1 chữ hoa ,1 chữ số và chữ thường!");

            RuleFor(x => x.PhoneNumber)
                 .NotEmpty().WithMessage("Số điện thoại không thể để trống");

            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.Password != request.RepeatPassword)
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
