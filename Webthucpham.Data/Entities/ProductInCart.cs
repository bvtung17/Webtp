using System;

namespace Webthucpham.Data.Entities
{
    public class ProductInCart
    {
        public Guid CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductName { get; set; }
        public Cart Cart { get; set; }
        public Product Product { get; set; }
    }
}