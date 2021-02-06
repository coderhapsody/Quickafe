using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Quickafe.DataAccess;
using Quickafe.Framework.Helpers;
using Quickafe.Providers;
using Quickafe.Providers.Sales;
using OrderViewModel = Quickafe.Providers.Sales.ViewModels.Order;
using Quickafe.Providers.Sales.ViewModels.Payment;
using Quickafe.ViewModels;
using Quickafe.Web.SessionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Areas.Sales.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentProvider paymentProvider;
        private readonly OrderProvider orderProvider;
        private readonly ConfigurationProvider configurationProvider;
        private readonly PaymentTypeProvider paymentTypeProvider;
        private readonly IMapper mapper;

        public PaymentController(
            PaymentProvider paymentProvider, 
            OrderProvider orderProvider,
            PaymentTypeProvider paymentTypeProvider, 
            ConfigurationProvider configurationProvider,
            IMapper mapper)
        {
            this.configurationProvider = configurationProvider;
            this.paymentProvider = paymentProvider;
            this.orderProvider = orderProvider;
            this.paymentTypeProvider = paymentTypeProvider;
            this.mapper = mapper;
        }

        public ActionResult CreatePayment(long orderId)
        {
            if (paymentProvider.IsPaid(orderId))
            {
                return RedirectToAction("Index", "Home", new { area = String.Empty });
            }

            Order order = orderProvider.GetOrder(orderId);
            var model = new CreatePaymentViewModel();

            var payableOrders = paymentProvider.CreatePaymentFromOrder(orderId);
            model.OrderDetails = payableOrders;

            //decimal serviceChargePercent = configurationProvider.GetConfiguration<decimal>(ConfigurationKeys.ServiceChargePercent);
            //model.ServiceChargeValue = (serviceChargePercent / 100) * model.TotalOrders;

            decimal taxPercent = configurationProvider.GetConfiguration<decimal>(ConfigurationKeys.TaxPercent);
            model.TaxValue = (taxPercent / 100) * (model.TotalOrders - order.DiscValue);

            model.DeliveryCharge = order.DeliveryCharge;
            model.DiscValue = order.DiscValue;
            model.DiscPercent = order.DiscPercent;

            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePayment(CreatePaymentViewModel model, FormCollection form)
        {
            long orderId = Convert.ToInt64(form["orderId"]);
            IEnumerable<PayableOrderViewModel> payableOrders = paymentProvider.CreatePaymentFromOrder(orderId);
            Order order = orderProvider.GetOrder(orderId);

            decimal deliveryCharge = order.DeliveryCharge;
            decimal discValue = order.DiscValue;
            decimal totalOrder = payableOrders.Sum(p => p.Quantity * p.UnitPrice - p.DiscValue);
            //decimal serviceChargePercent = configurationProvider.GetConfiguration<decimal>(ConfigurationKeys.ServiceChargePercent);
            //decimal serviceChargeValue = (serviceChargePercent / 100) * totalOrder;
            decimal taxPercent = configurationProvider.GetConfiguration<decimal>(ConfigurationKeys.TaxPercent);
            decimal taxValue = (taxPercent / 100) * (totalOrder - discValue);

            decimal billableAmount = totalOrder + /*serviceChargeValue +*/ deliveryCharge - discValue + taxValue;
            decimal totalPayment = PaymentDetailSessionWrapper.TotalPayment;

            if (totalPayment < billableAmount)
            {
                TempData["message"] = "Insufficient payment";
                return RedirectToAction("CreatePayment", new { orderId = orderId });
            }
            else
            {
                if (PaymentDetailSessionWrapper.TotalNonCash > billableAmount)
                {
                    TempData["message"] = "Non-cash payment(s) exceed billable amount";
                    return RedirectToAction("CreatePayment", new { orderId = orderId });
                }
            }

            var payment = new Payment()
            {
                BilledAmount = billableAmount,
                Date = DateTime.Today,
                OrderId = orderId,
                PaidAmount = totalPayment
            };
            var paymentDetail = new List<PaymentDetail>();
            mapper.Map(PaymentDetailSessionWrapper.Detail, paymentDetail);

            if(totalPayment > billableAmount)
            {
                decimal change = totalPayment - billableAmount;
                var paymentDetailChange = new PaymentDetail();
                paymentDetailChange.PaymentTypeId = 1;
                paymentDetailChange.Amount = change * -1;
                paymentDetailChange.Notes = "Change";
                paymentDetail.Add(paymentDetailChange);
            }

            payment.PaymentDetails.AddRange(paymentDetail);
            paymentProvider.AddPayment(order, payment);

            long paymentId = payment.Id;
            TempData["PaymentId"] = paymentId;

            OrderDetailSessionWrapper.Clear();
            PaymentDetailSessionWrapper.Clear();

            return RedirectToAction("Detail", new { id = paymentId, mode = StringHelper.XorString("confirmation") });
        }

        [HttpPost]
        public ActionResult GetTotalPayment()
        {
            var totalPayment = PaymentDetailSessionWrapper.TotalPayment;
            return Json(totalPayment);
        }

        public ActionResult AddDetail()
        {
            var model = new PaymentDetailViewModel();
            model.Amount = 0;
            return PartialView("PaymentDetail", model);
        }

        [HttpPost]
        public ActionResult AddDetail(PaymentDetailViewModel model)
        {
            PaymentDetailSessionWrapper.AddDetail(model);
            var ajaxViewModel = new AjaxViewModel(
               data: new { TotalPayment = PaymentDetailSessionWrapper.TotalPayment },
               message: "Payment detail has been updated.");
            return Json(ajaxViewModel);
        }

        [HttpGet]
        public ActionResult EditDetail(Guid detailUid)
        {
            var model = PaymentDetailSessionWrapper.GetDetail(detailUid);
            return PartialView("PaymentDetail", model);
        }

        [HttpPost]
        public ActionResult EditDetail(PaymentDetailViewModel model, FormCollection form)
        {
            PaymentDetailSessionWrapper.UpdateDetail(model);

            var ajaxViewModel = new AjaxViewModel(
                data: new { TotalPayment = PaymentDetailSessionWrapper.TotalPayment }, 
                message: "Payment detail has been updated.");
            return Json(ajaxViewModel);
        }

        public ActionResult DeleteDetail(Guid[] arrayOfId)
        {
            Array.ForEach(arrayOfId, PaymentDetailSessionWrapper.DeleteDetatil);
            var ajaxViewModel = new AjaxViewModel(
               data: new { TotalPayment = PaymentDetailSessionWrapper.TotalPayment },
               message: "Payment detail has been updated.");
            return Json(ajaxViewModel);
        }

        public ActionResult ListPaymentDetail([DataSourceRequest] DataSourceRequest request)
        {
            var paymentDetails = PaymentDetailSessionWrapper.Detail;
            return Json(paymentDetails.ToDataSourceResult(request));
        }

        public ActionResult GetPaymentType()
        {
            var paymentTypes = paymentTypeProvider.GetPaymentTypes(true);
            return Json(paymentTypes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult History()
        {
            var model = new HistoryViewModel();
            return View(model);
        }

        public ActionResult ListHistory([DataSourceRequest] DataSourceRequest request)
        {
            if(request.Sorts.Count == 0)
            {
                request.Sorts.Add(new Kendo.Mvc.SortDescriptor("PaymentDate", System.ComponentModel.ListSortDirection.Descending));
            }
            var historyList = paymentProvider.ListPaymentHistory();
            return Json(historyList.ToDataSourceResult(request));
        }

        public ActionResult GetPaymentTypes(int id)
        {
            string paymentTypes = paymentProvider.GetPaymentTypes(id);
            return Content(paymentTypes);
        }


        public ActionResult Detail(long id, string mode)
        {
            var model = new DetailViewModel();
            var detailModel = new List<PaymentDetailViewModel>();

            Payment payment = paymentProvider.GetPayment(id);
            var paymentDetail = paymentProvider.GetPaymentDetail(id);
            var orderDetail = orderProvider.GetOrderDetail(payment.OrderId);
            var order = orderProvider.GetOrder(payment.OrderId);

            mapper.Map(payment, model);
            mapper.Map(paymentDetail, detailModel);

            //model.ServiceCharge = order.ServiceCharge;
            model.TaxValue = order.TaxAmount;
            model.DeliveryCharge = order.DeliveryCharge;
            model.DiscValue = order.DiscValue;
            model.OrderId = order.Id;
            model.UnitPriceMode = orderProvider.GetUnitPriceMode(order.UnitPriceMode);
            model.List = detailModel;
            model.OrderDetails = mapper.Map<List<OrderViewModel.OrderDetailEntryViewModel>>(orderDetail);
            ViewBag.mode = mode.XorString();
            if (!IsDetailModeValid(ViewBag.mode))
            {
                return RedirectToAction("Index", "Home", new { area = String.Empty });
            }

            return View(model);
        }

        private bool IsDetailModeValid(string mode)
        {
            return new[] { "history", "confirmation" }.Contains(mode);
        }

        [HttpPost]
        public ActionResult IsAllowVoidPayment(string userName, string password)
        {
            return Json(new AjaxViewModel(paymentProvider.IsAllowVoidPayment(userName, password)));
        }

        [HttpPost]
        public ActionResult Void(long paymentId, string userName)
        {
            try
            {
                paymentProvider.VoidPayment(paymentId, userName);
                return Json(new AjaxViewModel(true));
            }
            catch (Exception ex)
            {
                return Json(new AjaxViewModel(false, message: ex.Message));
            }
        }

        public ActionResult Void(long id)
        {
            ViewBag.VoidAuthenticationUrl = Url.Action("IsAllowVoidPayment");
            ViewBag.VoidPaymentId = id;
            ViewBag.VoidUrl = Url.Action("Void");
            return PartialView();
        }
    }
}