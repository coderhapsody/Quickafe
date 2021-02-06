using Quickafe.Providers;
using Quickafe.Providers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Filters
{
    public class VerifyRoleAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string absolutePath = filterContext.HttpContext.Request.Url.AbsolutePath;
        }
    }
}