using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.ViewModels.System.Users
{
    public class UserViewModel
    {
        public Guid Id { get; set; }

        [Display(Name="Tên")]
        public string Name { get; set; }

        [Display(Name = "Số Điện Thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Tài Khoản")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Ngày Sinh")]
        public DateTime Dob { get; set; }

        public IList<string> Roles { get; set; }
        public List<SelectItem> RoleAssignRequest { get; set; }
    }
}
