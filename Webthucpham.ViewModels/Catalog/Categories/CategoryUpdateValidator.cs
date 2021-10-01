using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.Categories
{
    public class CategoryUpdateValidator : AbstractValidator<CategoryUpdateRequest>
    {
        public CategoryUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Không thể để trống trường này!");
        }
    }
}
