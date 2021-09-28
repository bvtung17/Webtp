using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.Products.Manage
{
    public class ProductEditRequest
    {
        public int Id { get; set; }
        public decimal newPrice { get; set; }
        public int addedStock { get; set; }
    }
}
