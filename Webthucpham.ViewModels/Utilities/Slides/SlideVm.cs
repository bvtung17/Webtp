﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.ViewModels.Utilities.Slide
{
    public class SlideVm
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Url { set; get; }
        public string Description { set; get; }
        public string Image { get; set; }
        public int SortOrder { set; get; }
       
    }
}
