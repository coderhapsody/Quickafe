using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Quickafe.Framework.Base
{
    public class BaseController : Controller
    {
        protected const string XorKey = "Quickafe2016";

        public ILog Logger { get; set; }

        public virtual ClaimsPrincipal Principal { get; set; }

        protected virtual string CurrentUserName =>
           Principal == null ? "guest" : Principal.Identity.Name;

        protected virtual JsonNetResult JsonNetResult(object  data)
        {
            return new Framework.JsonNetResult() { Data = data };
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if(Logger != null)
            {
                HostingEnvironment.QueueBackgroundWorkItem(ct => Logger.Error(filterContext.Exception));                
            }
            base.OnException(filterContext);
        }

        public JsonResult HandleException(Exception ex)
        {
            if (Logger != null)
            {
                Logger.Error(ex);
            }
            return Json(new BaseAjaxViewModel(false, ex.Message));
        }
    }
}
