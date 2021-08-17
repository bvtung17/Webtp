using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.ViewModels.System.Users
{
    public class GetUserPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}
