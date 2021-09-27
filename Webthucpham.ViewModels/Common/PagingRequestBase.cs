using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Common
{
    public class PagingRequestBase
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}

