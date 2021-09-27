using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Enums;

namespace Webthucpham.ViewModels.Catalog.Categories
{
    public class CategoryVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool IsOutstanding { get; set; }
        public int? ParentId { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
