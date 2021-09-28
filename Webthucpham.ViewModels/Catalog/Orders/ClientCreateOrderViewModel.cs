using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.ViewModels.Catalog.Carts;
using Webthucpham.ViewModels.System.Clients;

namespace Webthucpham.ViewModels.Catalog.Orders
{
    public class ClientCreateOrderViewModel
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
