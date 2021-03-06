using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Categories;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Models
{
    public class ProductCategoryViewModel
    {
        public CategoryVm Category { get; set; }
        public PagedResult<ProductVm> Products { get; set; }
    }
}
