using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Sales
{
    public class OrderCreateRequest
    {
        public int OrderId { get; set; }
        public string ClientName { get; set; }
        public string ClientNote { get; set; }
        public Guid? ClientID { get; set; }
        public string Email { get; set; }
        public string ShipAddress { get; set; }
        public string ShipPhone { get; set; }
        public bool SaveShipInfo { get; set; }
        public decimal TotalPrice { get; set; }
        public ClientCartViewModel ClientCart { get; set; }
        public ClientUpdateViewModel Client { get; set; }
    }
}
