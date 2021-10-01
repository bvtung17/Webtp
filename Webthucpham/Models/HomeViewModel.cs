    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Catalog.Categories;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Utilities.Slide;

namespace Webthucpham.Models
{
    public class HomeViewModel
    {
        public List<SlideVm> Slides { get; set; }
        //public List<BannerViewModel> Banners { get; set; }
        public List<HomeCategoryViewModel> Categories { get; set; }
    }
}
