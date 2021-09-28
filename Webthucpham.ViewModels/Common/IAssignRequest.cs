using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Common
{
    public class IAssignRequest<T, Q>
    {
        public T Id;
        public List<SelectItemDynamic<Q>> ItemsRequest { get; set; }
    }
}
