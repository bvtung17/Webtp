using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Catalog.Products.Manage;

namespace Webthucpham.ViewModels.Catalogs.Products
{
    public class CreateProductValidator : AbstractValidator<ProductCreateRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage("Tên sản phẩm không được để trống")
                .MaximumLength(200).WithMessage("Tên sản phẩm tối đa là 200 ký tự");

            RuleFor(x => x.Price).NotEmpty()
                .WithMessage("Giá bán không được để trống").GreaterThan(10000).WithMessage("Giá bán phải lớn hơn 10.000");

            RuleFor(x => x.ThumbnailImage).NotEmpty()
                .WithMessage("Ảnh không được để trống");

            RuleFor(x => x.OriginalPrice).NotEmpty()
                .WithMessage("Giá nhập vào không thể để trống").GreaterThan(9999).WithMessage("Giá nhập phải lơn hơn hoặc bằng 10.000");

            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.OriginalPrice < request.Price)
                {
                    context.AddFailure("Giá bán không được để lớn hơn giá nhập");
                }

            });

            RuleFor(x => x.Stock).NotEmpty().WithMessage("Số lượng trong kho không được để trống").GreaterThan(-1).WithMessage("Số lượng không được âm");

        }
    }
}
