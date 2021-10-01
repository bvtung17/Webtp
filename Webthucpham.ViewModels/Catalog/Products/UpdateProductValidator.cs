using FluentValidation;
using Webthucpham.ViewModels.Catalog.Products.Manage;

namespace Webthucpham.ViewModels.Catalogs.Products
{
    public class UpdateProductValidator : AbstractValidator<ProductUpdateRequest>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage("Tên sản phẩm không được để trống")
                .MaximumLength(200).WithMessage("Tên sản phẩm không được dài hơn 200 ký tự");

            RuleFor(x => x.Price).NotEmpty()
                .WithMessage("Giá bán không được để trống").GreaterThan(9999).WithMessage("Giá bán không được nhỏ hơn 10.000");

            
            RuleFor(x => x.OriginalPrice).NotEmpty()
                .WithMessage("Giá nhập không được đẻ trống").WithMessage("Giá nhập không được nhỏ hơn 10.000");

            RuleFor(x => x.SelectedId).NotEmpty()
                .WithMessage("Danh mục của sản phẩm không được để trống");

            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.OriginalPrice < request.Price)
                {
                    context.AddFailure("Giá bán không được lớn hơn giá nhập");
                }
            });

            RuleFor(x => x.Stock).NotEmpty().WithMessage("Số lượng trong kho không được để trống").GreaterThan(-1)
                .WithMessage("Số lượng trong kho không được âm");

        }
    }
}
