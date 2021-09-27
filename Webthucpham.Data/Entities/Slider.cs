using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Enums;

namespace Webthucpham.Data.Entities
{
    public class Slider
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Url { set; get; }
        public string Description { set; get; }
        public string Image { get; set; }
        public int SortOrder { set; get; }
        public Status Status { set; get; }

    }
}
