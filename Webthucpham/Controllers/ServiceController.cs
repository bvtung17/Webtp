using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Controllers
{
    public class ServiceController : Controller
    {
        [HttpPost]
        public JsonResult Index(ServiceViewModel service)
        {
            if (!String.IsNullOrEmpty(service.SessionName) && !String.IsNullOrEmpty(service.SessionValue))
            {
                HttpContext.Session.SetString(service.SessionName, service.SessionValue);
            }

            return new JsonResult(new { result = 1 });
        }
    }
}
