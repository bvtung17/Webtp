using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Enums;

namespace Webthucpham.Data.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public DateTime Dob { get; set; }
        public Status Status { get; set; }
        public List<Cart> Carts { get; set; }
        public List<Order> Orders { get; set; }

    }
}
