using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.ProductImages
{
    public class ProductImageCreateValidator : AbstractValidator<ProductImageCreateRequest>
    {
        public ProductImageCreateValidator()
        {
            RuleFor(x => x.ImageFile).NotEmpty()
                .WithMessage("Ảnh sản phẩm không được để trống");

            RuleFor(x => x.Caption).NotEmpty()
                .WithMessage("Chú thích không được để trống");
        }
    }
}
