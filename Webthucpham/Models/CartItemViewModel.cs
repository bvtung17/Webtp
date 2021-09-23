using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webthucpham.Models
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Desription { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }
}
