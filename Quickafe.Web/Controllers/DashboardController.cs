using Quickafe.Providers;
using Quickafe.Providers.ViewModels.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ChartProvider chartProvider;

        public DashboardController(ChartProvider chartProvider)
        {
            this.chartProvider = chartProvider;    
        }

        public ActionResult MtdOrderVolume()
        {
            var model = chartProvider.GetMtmOrderVolume(DateTime.Today.Year);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}