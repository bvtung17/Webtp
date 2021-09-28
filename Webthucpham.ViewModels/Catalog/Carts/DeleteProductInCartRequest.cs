using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.Carts
{
    public class DeleteProductInCartRequest
    {
        public int ProductId { get; set; }
        public ClientCartViewModel Cart { get; set; }
    }
}
