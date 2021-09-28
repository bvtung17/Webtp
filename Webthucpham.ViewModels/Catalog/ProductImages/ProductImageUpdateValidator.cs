using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.ProductImages
{
    public class ProductImageUpdateValidator : AbstractValidator<ProductImageUpdateRequest>
    {
        public ProductImageUpdateValidator()
        {
            RuleFor(x => x.Caption).NotEmpty()
                .WithMessage("Chú thích không được để trống");
        }
    }
}
