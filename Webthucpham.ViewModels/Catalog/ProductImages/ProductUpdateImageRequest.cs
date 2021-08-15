using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.ProductImages
{
   public class ProductUpdateImageRequest
    {
        public int Id { get; set; }
        
       
        public string Caption { get; set; }
        public bool IsDefault { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
