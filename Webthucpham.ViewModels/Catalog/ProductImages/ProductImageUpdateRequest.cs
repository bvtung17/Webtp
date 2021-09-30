using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.ProductImages
{
   public class ProductImageUpdateRequest
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Caption { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
