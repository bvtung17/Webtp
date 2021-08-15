using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.ViewModels.Catalog.ProductImages
{
    public class GetPublicProductPagingRequest : PagingRequestBase
    {
        public int? CategoryId { get; set; }
    }
}
