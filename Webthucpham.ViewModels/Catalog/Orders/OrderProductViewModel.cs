using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.Orders
{
    public class OrderProductViewModel
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
