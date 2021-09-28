using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.System.Clients
{
    public class ClientUpdateViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime Dob { get; set; }
        public string OldPassword { get; set; }
        public string Avatar { get; set; }
        public IFormFile NewAvatar { get; set; }
        public string PhoneNumber { get; set; }
        public string NewPassword { get; set; }
        public string RepeatPassword { get; set; }
    }
}
