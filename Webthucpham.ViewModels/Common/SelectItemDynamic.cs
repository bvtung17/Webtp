using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Common
{
    public class SelectItemDynamic<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}
