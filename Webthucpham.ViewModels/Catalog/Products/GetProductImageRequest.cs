using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.Products
{
    public class GetProductImageRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Images { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}
