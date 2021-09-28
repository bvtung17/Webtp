
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Webthucpham.Data.Enums;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.ViewModels.Catalog.Products
{
    public class ProductCreateRequest
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        [DisplayName("Original Country")]
        public string OriginalCountry { get; set; }
        public decimal Price { get; set; }
        [DisplayName("Original Price")]
        public decimal OriginalPrice { get; set; }
        public string Details { get; set; }
        public int Stock { get; set; }
        public int ViewCount { get; set; }
        public DateTime DateCreated { get; set; }
        [DisplayName("Is featured")]
        public bool IsOutstanding { get; set; } 
        [DisplayName("Thumbnail")]
        public IFormFile ThumbnailImage { get; set; }
        public List<SelectItemDynamic<int>> CategoriesAssignRequest { get; set; }
     = new List<SelectItemDynamic<int>>();

        public Status status { get; set; }
    }
}
