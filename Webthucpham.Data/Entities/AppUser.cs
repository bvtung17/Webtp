using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.Data.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        // bổ xung thêm cho toàn hệ thống
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }

        public List<Cart> Carts { get; set; } // with Many Cart
        public List<Order> Orders { get; set; } // with Many Order
        public List<Transaction> Transactions { get; set; } // with Many Transaction
    }
}
