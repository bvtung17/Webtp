using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Sales
{
    public class OrderCreateRequest
    {
        public DateTime OrderDate { set; get; }
        public Guid UserId { set; get; }
        public string ShipName { set; get; }
        public string ShipAddress { set; get; }
        public string ShipEmail { set; get; }
        public string ShipPhoneNumber { set; get; }
        public int Status { set; get; }
        public List<KeyValuePair<int, int>> ProductDetails { get; set; }
        public decimal Price { set; get; }
    }
}
