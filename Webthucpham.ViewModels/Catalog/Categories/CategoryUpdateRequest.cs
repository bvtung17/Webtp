using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Catalog.Categories
{
    public class CategoryUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOutstanding { get; set; }
    }
}
