using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Quickafe.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Controllers
{
    public class BrowseController : Controller
    {
        private readonly ProductProvider productProvider;

        public BrowseController(ProductProvider productProvider)
        {
            this.productProvider = productProvider;
        }

        #region Browse Product
        public ActionResult BrowseProduct(string callback, string inventoryType, int? unitPriceMode)
        {
            ViewBag.callback = callback;
            ViewBag.inventoryType = inventoryType;
            ViewBag.unitPriceMode = unitPriceMode;
            return PartialView();
        }

        public ActionResult ListBrowseProduct([DataSourceRequest] DataSourceRequest request, string inventoryType, int? unitPriceMode)
        {
            unitPriceMode = unitPriceMode ?? -1;
            var list = productProvider.ListProducts(finishedGoods:inventoryType == "F", unitPriceMode: unitPriceMode.Value);
            return Json(list.ToDataSourceResult(request));
        }
        #endregion
    }
}