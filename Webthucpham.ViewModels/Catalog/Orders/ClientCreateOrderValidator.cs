using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Webthucpham.ViewModels.Catalog.Orders
{
    public class ClientCreateOrderValidator : AbstractValidator<ClientCreateOrderViewModel>
    {
        public ClientCreateOrderValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email không được để trống")
                .MaximumLength(100).WithMessage("Email không thể quá 100 ký tự")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Không đúng định dạng email!");
            RuleFor(x => x.ShipAddress).NotEmpty().WithMessage("Địa chỉ giao hàng không thể để trống");
            RuleFor(x => x.ShipPhone).NotEmpty().WithMessage("Số điện thoại không thể để trống");
            RuleFor(x => x.ClientName).NotEmpty().WithMessage("Số điện thoại không thể để trống");
            RuleFor(x => x).Custom((request, context) =>
            {
                var regexString = @"(84|0[3|5|7|8|9])+([0-9]{8})";
                var regex = new Regex(regexString);
                if (request.ShipPhone == null)
                {
                    context.AddFailure("Số điện thoại không được để trống");
                }
                if (request.ShipPhone == null && !regex.IsMatch(request.ShipPhone ?? "")) context.AddFailure("Số điện thoại không đúng định dạng");
            });
        }
    }
}
