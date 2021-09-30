using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Webthucpham.Data.Entities;
using Webthucpham.Data.Enums;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.ViewModels.Catalog.Products.Manage
{
    public class ProductUpdateRequest
    {
        public int Id { get; set; }
        //public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public decimal Price { get; set; }
        [DisplayName("Original price")]
        public decimal OriginalPrice { get; set; }
        [DisplayName("Date created")]
        public DateTime DateCreated { get; set; }
        public int Stock { get; set; }
        [DisplayName("View count")]

        public int ViewCount { get; set; }
        [DisplayName("Original country")]
        public string OriginalCountry { get; set; }
        public IList<string> Categories { get; set; }
        public List<SelectItemDynamic<int>> CategoriesAssignRequest { get; set; }
        = new List<SelectItemDynamic<int>>();
        public IEnumerable<Category> CategoryList { get; set; }
        public string[] SelectedId { get; set; }
        public string ImagePath { get; set; }
        public IFormFile ThumbnailImage { get; set; }
        public Status status { get; set; }
    }
}
