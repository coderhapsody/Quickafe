using Quickafe.Providers;
using Quickafe.Providers.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly ProductCategoryProvider productCategoryProvider;

        public ReportController(ProductCategoryProvider productCategoryProvider)
        {
            this.productCategoryProvider = productCategoryProvider;
        }

        // GET: Report
        public ActionResult DailySales()
        {
            return View();
        }

        public ActionResult ProductSalesByCategory()
        {
            ViewBag.FromDate = DateTime.Today;
            ViewBag.ToDate = DateTime.Today;
            return View();
        }

        public ActionResult StockCard()
        {
            ViewBag.FromDate = DateTime.Today;
            ViewBag.ToDate = DateTime.Today;
            return View();
        }

        public ActionResult StockMutation()
        {
            ViewBag.FromDate = DateTime.Today;
            ViewBag.ToDate = DateTime.Today;
            return View();
        }

        public ActionResult GoFoodOrderList()
        {
            ViewBag.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            ViewBag.ToDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            return View();
        }

        public ActionResult OrderFrequency()
        {
            ViewBag.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            ViewBag.ToDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            return View();
        }
    }
}