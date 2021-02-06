using AutoMapper;
using Castle.Core.Internal;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Quickafe.DataAccess;
using Quickafe.Providers;
using Quickafe.ViewModels;
using Quickafe.Providers.ViewModels.PaymentType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quickafe.Framework.Base;

namespace Quickafe.Web.Areas.Master.Controllers
{
    public class PaymentTypeController : BaseController
    {
        private readonly PaymentTypeProvider paymentTypeProvider;
        private readonly IMapper mapper;

        public PaymentTypeController(PaymentTypeProvider productCategoryProvider, IMapper mapper)
        {
            this.mapper = mapper;
            this.paymentTypeProvider = productCategoryProvider;
        }

        public ActionResult Index()
        {
            var model = new IndexViewModel();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new CreateEditViewModel();
            return PartialView("CreateEdit", model);
        }

        public ActionResult Edit(long id)
        {
            var model = new CreateEditViewModel();
            var productCategory = paymentTypeProvider.GetPaymentType(id);
            mapper.Map(productCategory, model);

            return PartialView("CreateEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(CreateEditViewModel model)
        {
            var paymentType = new PaymentType();
            mapper.Map(model, paymentType);
            try
            {
                paymentTypeProvider.UpdatePaymentType(paymentType);
            }
            catch(Exception ex)
            {
                return HandleException(ex);                
            }
            var jsonViewModel = new AjaxViewModel(true, model, null);
            return Json(jsonViewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateEditViewModel model)
        {
            var paymentType = new PaymentType();
            mapper.Map(model, paymentType);
            try
            {
                paymentTypeProvider.AddPaymentType(paymentType);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
            var jsonViewModel = new AjaxViewModel(true, model, null);
            return Json(jsonViewModel);
        }

        public ActionResult List([DataSourceRequest] DataSourceRequest request)
        {
            var list = paymentTypeProvider.ListPaymentTypes();
            return Json(list.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Delete(IEnumerable<long> arrayOfId)
        {
            try
            {
                arrayOfId.ForEach(paymentTypeProvider.DeletePaymentType);
                return Json(new AjaxViewModel(true, null, null));
            }
            catch (Exception ex)
            {
                return Json(new AjaxViewModel(false, null, ex.Message));
            }
        }
    }
}