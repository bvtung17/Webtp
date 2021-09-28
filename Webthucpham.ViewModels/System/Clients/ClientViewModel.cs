using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.ViewModels.Sales;

namespace Webthucpham.ViewModels.System.Clients
{
    public class ClientViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime Dob { get; set; }
        public string Avatar { get; set; }
        public IFormFile NewAvatar { get; set; }
        public string PhoneNumber { get; set; }
        public int OrderQuanttity { get; set; }
        public List<OrderViewModel> Orders { get; set; }
    }
}
