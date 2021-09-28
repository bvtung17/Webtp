using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Common
{
    public class PaginateRequest :PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}
