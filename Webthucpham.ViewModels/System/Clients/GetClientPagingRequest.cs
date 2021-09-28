using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.ViewModels.System.Clients
{
    public class GetClientPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}
