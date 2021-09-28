using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Enums;

namespace Webthucpham.ViewModels.Catalog.Orders
{
    public class ClientOrderHistoryViewMode
    {
        public int Id { set; get; }
        public Guid ClientId { set; get; }
        public DateTime OrderDate { set; get; }
        public decimal Total { set; get; }
        public OrderStatus Status { set; get; }
    }
}
