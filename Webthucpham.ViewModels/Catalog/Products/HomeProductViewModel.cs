using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.ViewModels.Catalog.ProductImages;

namespace Webthucpham.ViewModels.Catalog.Products
{
    public class HomeProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public List<ProductImageViewModel> Images { get; set; }
    }
}
