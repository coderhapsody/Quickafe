using Quickafe.Providers;
using Quickafe.Providers.ViewModels.LookUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Areas.Master.Controllers
{
    public class LookUpController : Controller
    {
        private readonly LookUpProvider lookUpProvider;

        public LookUpController(LookUpProvider lookUpProvider)
        {
            this.lookUpProvider = lookUpProvider;
        }

        public ActionResult Index()
        {
            var model = new IndexViewModel();
            model.List = lookUpProvider.GetLookUpNames();
            return View(model);
        }

        public ActionResult Edit(string name)
        {
            IEnumerable<string> lookUpValues = lookUpProvider.GetLookUpValues(name);
            string values = String.Join("\n", lookUpValues);
            ViewBag.values = values;
            return View();
        }

        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            string lookUpName = form["lookUpName"];
            string lookUpValues = form["lookUpValues"];
            lookUpProvider.SaveLookUpValues(lookUpName, lookUpValues.Replace("\r\n",","));
            return RedirectToAction("Index");
        }
      
    }
}