using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.ViewModels.Catalog.Products.Public
{
    public class PublicPagingRequest : PagingRequestBase
    {
        public int? CategoryId { get; set; }
    }
}
