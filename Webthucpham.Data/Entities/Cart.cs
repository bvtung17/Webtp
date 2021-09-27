using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.Data.Entities
{
    
        public class Cart
        {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid? ClientId { get; set; }
        public DateTime DateCreated { get; set; }
        public List<ProductInCart> ProductInCarts { get; set; }
        public Client Client { get; set; }
    }
}
