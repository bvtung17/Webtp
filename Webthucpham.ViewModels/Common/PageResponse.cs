using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Common
{
    public class PageResponse<T> : PagedResultBase
    {
        public List<T> Items { get; set; } = new List<T>();

    }
}
