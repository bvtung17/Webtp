using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Application.Dtos;

namespace Webthucpham.Application.Catalog.Products.Dtos.Public
{
    public class GetProductPagingRequest :PagingRequestBase
    {
        public int CategoryId { get; set; }
    }
}
