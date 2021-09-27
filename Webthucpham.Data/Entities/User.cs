using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        // bổ xung thêm cho toàn hệ thống
        public string Name { get; set; }
      public DateTime Dob { get; set; }
    }
}
