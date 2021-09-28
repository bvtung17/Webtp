using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.System.Clients
{
    public class ClientRegisterRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Dob { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }
}
