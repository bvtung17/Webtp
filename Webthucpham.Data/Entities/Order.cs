using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Enums;

namespace Webthucpham.Data.Entities
{
    public class Order
    {
        public int Id { set; get; }
        public DateTime OrderDate { set; get; }
        public Guid? ClientId { set; get; }
        public Guid? CartId { set; get; }
        public string ShipName { set; get; }
        public string ShipAddress { set; get; }
        public string ShipEmail { set; get; }
        public string Note { set; get; }
        public string ShipPhoneNumber { set; get; }
        public string CancelReason { set; get; }
        public decimal Price { set; get; }
        public OrderStatus Status { set; get; }
        public List<OrderDetail> OrderDetails { set; get; }
        public Client Client { set; get; }

    }
}
