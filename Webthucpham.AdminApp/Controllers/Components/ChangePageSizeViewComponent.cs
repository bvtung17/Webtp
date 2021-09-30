using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.AdminApp.Controllers.Components
{
    public class ChangePageSizeViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(PageResponseBase request)
        {
            return Task.FromResult((IViewComponentResult)View("ChangePageSize", request));
        }

    }
}
