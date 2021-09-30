using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.ViewModels.Catalog.Products;

namespace Webthucpham.ViewModels.Catalog.Categories
{
    public class HomeCategoryViewModel
    {
        public List<HomeProductViewModel> Products { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
    }
}
