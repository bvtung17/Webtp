using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.Data.Entities
{
    public class AppRole :IdentityRole<Guid>
    {
        // bổ sung thêm trường
        public string Description { get; set; }
        
    }
}
