using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.Carts
{
    public class ClientCartViewModel
    {
        public Guid Id { get; set; }
        public Guid? ClientId { get; set; }
        public decimal CartPrice { get; set; }
        public List<ProductInCartViewModel> Products { get; set; } = new List<ProductInCartViewModel>();
    }
}
