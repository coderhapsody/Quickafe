using Quickafe.Providers;
using Quickafe.Providers.ViewModels.Configuration;
using Quickafe.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Areas.Master.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ConfigurationProvider configurationProvider;

        public ConfigurationController(ConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        [NonAction]
        private ConfigurationViewModel GetConfigurations()
        {
            var model = new ConfigurationViewModel();

            model.OrderNumberingLength = configurationProvider.GetConfiguration<int>(ConfigurationKeys.OrderNumberingLength);
            model.OrderPrefix = configurationProvider[ConfigurationKeys.OrderPrefix];
            model.PaymentNumberingLength = configurationProvider.GetConfiguration<int>(ConfigurationKeys.PaymentNumberingLength);
            model.PaymentPrefix = configurationProvider[ConfigurationKeys.PaymentPrefix];
            model.ServiceChargePercent = configurationProvider.GetConfiguration<decimal>(ConfigurationKeys.ServiceChargePercent);
            model.StoreName = configurationProvider[ConfigurationKeys.StoreName];
            model.TaxPercent = configurationProvider.GetConfiguration<decimal>(ConfigurationKeys.TaxPercent);
            model.UnitPriceLabel = configurationProvider[ConfigurationKeys.UnitPriceMode1];
            model.UnitPrice2Label = configurationProvider[ConfigurationKeys.UnitPriceMode2];
            model.UnitPrice3Label = configurationProvider[ConfigurationKeys.UnitPriceMode3];
            return model;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = GetConfigurations();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ConfigurationViewModel model, FormCollection form)
        {
            configurationProvider.UpdateConfiguration(ConfigurationKeys.OrderNumberingLength, model.OrderNumberingLength);
            configurationProvider.UpdateConfiguration(ConfigurationKeys.OrderPrefix, model.OrderPrefix);
            configurationProvider.UpdateConfiguration(ConfigurationKeys.PaymentNumberingLength, model.PaymentNumberingLength);
            configurationProvider.UpdateConfiguration(ConfigurationKeys.PaymentPrefix, model.PaymentPrefix);
            configurationProvider.UpdateConfiguration(ConfigurationKeys.ServiceChargePercent, model.ServiceChargePercent);
            configurationProvider.UpdateConfiguration(ConfigurationKeys.TaxPercent, model.TaxPercent);
            configurationProvider.UpdateConfiguration(ConfigurationKeys.StoreName, model.StoreName);
            configurationProvider.UpdateConfiguration(ConfigurationKeys.UnitPriceMode1, model.UnitPriceLabel);
            configurationProvider.UpdateConfiguration(ConfigurationKeys.UnitPriceMode2, model.UnitPrice2Label);
            configurationProvider.UpdateConfiguration(ConfigurationKeys.UnitPriceMode3, model.UnitPrice3Label);

            var ajaxViewModel = new AjaxViewModel();
            return Json(ajaxViewModel);
        }
    }
}