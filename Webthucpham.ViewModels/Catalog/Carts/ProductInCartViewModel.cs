using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.Carts
{
    public class ProductInCartViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
    }
}
