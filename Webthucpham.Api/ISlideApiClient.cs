using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Utilities.Slide;

namespace Webthucpham.Api
{
    public interface ISlideApiClient
    {
        Task<List<SlideVm>> GetAll();
    }
}
 