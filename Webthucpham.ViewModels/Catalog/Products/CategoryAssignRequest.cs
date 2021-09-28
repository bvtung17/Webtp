using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.ViewModels.Catalog.Products
{
    public class CategoryAssignRequest
    {
        public int Id { get; set; }
        public List<SelectItemDynamic<int>> Categories { get; set; } = new List<SelectItemDynamic<int>>();
        public string[] SelectedCategories { get; set; }
        public IEnumerable<SelectItemDynamic<int>> ListCategory
        {
            get
            {
                return Categories;
            }
        }
    }
}
