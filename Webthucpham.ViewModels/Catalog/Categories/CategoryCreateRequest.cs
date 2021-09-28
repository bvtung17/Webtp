using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.Categories
{
    public class CategoryCreateRequest
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }
}
