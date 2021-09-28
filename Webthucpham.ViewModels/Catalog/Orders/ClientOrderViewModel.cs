using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Enums;

namespace Webthucpham.ViewModels.Catalog.Orders
{
    public class ClientOrderViewModel
    {
        public int Id { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipEmail { get; set; }
        public string Note { get; set; }
        public string ShipPhoneNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
    }
}
