using AutoMapper;
using Quickafe.Providers.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quickafe.Providers.Sales.ViewModels.Order;
using Quickafe.Providers;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ProductViewModels = Quickafe.Providers.ViewModels.Product;
using Quickafe.ViewModels;
using Quickafe.Web.SessionWrappers;
using Quickafe.DataAccess;
using Quickafe.Framework;
using Quickafe.Framework.Helpers;
using Quickafe.Framework.Base;

namespace Quickafe.Web.Areas.Sales.Controllers
{
    public class OrderController : BaseController
    {
        private readonly ProductProvider productProvider;
        private readonly LookUpProvider lookUpProvider;
        private readonly OrderProvider orderProvider;
        private readonly TableProvider tableProvider;
        private readonly ConfigurationProvider configurationProvider;
        private readonly IMapper mapper;

        public OrderController(
            LookUpProvider lookUpProvider, 
            OrderProvider orderProvider, 
            TableProvider tableProvider, 
            ConfigurationProvider configurationProvider,
            ProductProvider productProvider,
            IMapper mapper)
        {
            this.productProvider = productProvider;
            this.lookUpProvider = lookUpProvider;
            this.orderProvider = orderProvider;
            this.tableProvider = tableProvider;
            this.configurationProvider = configurationProvider;
            this.mapper = mapper;
        }

        // GET: Sales/Order
        public ActionResult Index()
        {
            var model = new IndexViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            if (form["btnCreatePayment"] != null)
            {
                // Create Invoice for selected orders.
                if (form.GetValues("chkSelect") != null && form.GetValues("chkSelect").Length > 0)
                {
                    string[] selectedOrders = form.GetValues("chkSelect");
                    long[] billableOrders = Array.ConvertAll(selectedOrders, Convert.ToInt64);
                    TempData["billableOrders"] = billableOrders;
                    return RedirectToAction("CreatePayment", "Payment", new { area = "Sales" });
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ProductLookUp(string productCode)
        {
            try
            {
                Product product = productProvider.GetProduct(productCode);
                return Json(new AjaxViewModel(data: product), JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new AjaxViewModel(isSuccess: false, data: ex, message: ex.Message));
            }
        }

        public ActionResult ListOutstandingOrders([DataSourceRequest] DataSourceRequest request)
        {
            if (!request.Sorts.Any())
                request.Sorts.Add(new Kendo.Mvc.SortDescriptor("ChangedWhen", System.ComponentModel.ListSortDirection.Descending));
            var orderList = orderProvider.ListOutstandingOrders();
            return Json(orderList.ToDataSourceResult(request));
        }

        public List<DropDownListItem> GetUnitPriceModes()
        {
            var list = new List<DropDownListItem>();
            var unitPriceModes = orderProvider.GetUnitPriceModes();
            foreach(var unitPriceMode in unitPriceModes)
                list.Add(new DropDownListItem() { Text = unitPriceMode.Value, Value = unitPriceMode.Key.ToString() });

            return list;
        }

        public ActionResult Create(int unitPriceMode = 0)
        {
            var model = new CreateEditViewModel();
            model.OrderNo = "NEW";
            model.Date = DateTime.Today;
            model.Start = DateTime.Now;
            model.UnitPriceMode = unitPriceMode;

            var orderTypes = lookUpProvider.GetLookUpValues("OrderType").ToList();
            ViewBag.DefaultOrderType = orderTypes[0];

            OrderDetailSessionWrapper.Initialize(); 
            return View("CreateEdit", model);
        }

        [HttpPost]
        public ActionResult ValidateOrder(CreateEditViewModel model, FormCollection form)
        {
            AjaxViewModel validationResult;
            if (OrderDetailSessionWrapper.IsEmpty)
            {
                validationResult = new AjaxViewModel(isSuccess: false, message: "Order Detail is Empty");
                return Json(validationResult);
            }
            validationResult = new AjaxViewModel(isSuccess: true);
            return Json(validationResult);
        }

        [HttpPost]
        public ActionResult Create(CreateEditViewModel model, FormCollection form)
        {
            var order = new Order();
            var orderDetails = new List<OrderDetail>();
            mapper.Map(model, order);
            mapper.Map(OrderDetailSessionWrapper.Detail, orderDetails);
            order.LocationId = 1;
            order.DiscValue = order.DiscPercent > 0 ? order.DiscPercent / 100 * OrderDetailSessionWrapper.TotalOrder : order.DiscValue;
            order.OrderDetails.AddRange(orderDetails);

            CalculateOrderChargesAndDiscount(order, OrderDetailSessionWrapper.TotalOrder);

            orderProvider.AddOrder(order);
            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        public ActionResult Edit(long id)
        {
            var orderViewModel = new CreateEditViewModel();
            var orderDetailViewModel = new List<OrderDetailEntryViewModel>();
            var order = orderProvider.GetOrder(id);
            if (order != null)
            {
                var orderDetail = orderProvider.GetOrderDetail(order.Id);
                if (orderDetail != null)
                {
                    mapper.Map(order, orderViewModel);
                    mapper.Map(orderDetail, orderDetailViewModel);
                    OrderDetailSessionWrapper.Initialize(orderDetailViewModel);
                    return View("CreateEdit", orderViewModel);
                }
            }
            return View("Index");
        }

        private void CalculateOrderChargesAndDiscount(Order order, decimal totalOrder)
        {
            decimal taxPercent = configurationProvider.GetConfiguration<decimal>(ConfigurationKeys.TaxPercent);
            //decimal serviceChargePercent = configurationProvider.GetConfiguration<decimal>(ConfigurationKeys.ServiceChargePercent);
            //decimal serviceCharge = serviceChargePercent / 100 * totalOrder;
            decimal discValue = order.DiscPercent > 0 ? order.DiscPercent / 100 * OrderDetailSessionWrapper.TotalOrder : order.DiscValue;
            decimal taxValue = taxPercent / 100 * (totalOrder - discValue);

            order.TaxAmount = taxValue;
            //order.ServiceCharge = serviceCharge;
            order.DiscValue = discValue;
        }

        [HttpPost]
        public ActionResult Edit(CreateEditViewModel model, FormCollection form)
        {
            var order = orderProvider.GetOrder(model.Id);
            var orderDetails = new List<OrderDetail>();
            mapper.Map(model, order);
            mapper.Map(OrderDetailSessionWrapper.Detail, orderDetails);

            CalculateOrderChargesAndDiscount(order, OrderDetailSessionWrapper.TotalOrder);

            order.OrderDetails.Clear();           
            foreach(OrderDetail orderDetail in orderDetails)
            {
                orderDetail.OrderId = order.Id;
                order.OrderDetails.Add(orderDetail);
            }
            orderProvider.UpdateOrder(order);
            return RedirectToAction("OrderConfirmation", new { orderId = model.Id });
        }

        public ActionResult OrderConfirmation(long orderId)
        {
            if(orderId > 0)
            {
                return RedirectToAction("Detail", new { id=orderId, mode=StringHelper.XorString("confirmation") });
            }

            return RedirectToAction("Index", "Home", new { area = String.Empty });
        }

        public ActionResult BrowseProduct()
        {
            var productList = new List<ProductViewModels.ListProductViewModel>();
            return PartialView("BrowseProduct", productList);
        }

        public ActionResult GetOrderTypes()
        {
            var orderTypes = lookUpProvider.GetLookUpValues("OrderType");
            return Json(orderTypes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPricingModes()
        {
            var pricingModes = GetUnitPriceModes();
            return Json(pricingModes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTables(bool isUpdating)
        {
            var tables = tableProvider.GetTables(!isUpdating);
            return Json(tables, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateDetail()
        {
            var model = new OrderDetailEntryViewModel();
            model.Qty = 1;
            return PartialView("CreateEditDetail", model);
        }

        [HttpPost]
        public ActionResult CreateDetail(OrderDetailEntryViewModel model, FormCollection form)
        {
            OrderDetailSessionWrapper.AddDetail(model);
            var ajaxViewModel = new AjaxViewModel(message: "Order detail has been added.");
            return Json(ajaxViewModel);
        }

        [HttpGet]
        public ActionResult EditDetail(Guid detailUid)
        {
            //Guid detailUid = Guid.Parse(detailUidString);
            var model = OrderDetailSessionWrapper.GetDetail(detailUid);
            return PartialView("CreateEditDetail", model);
        }

        [HttpPost]
        public ActionResult EditDetail(OrderDetailEntryViewModel model, FormCollection form)
        {
            OrderDetailSessionWrapper.UpdateDetail(model);
            var ajaxViewModel = new AjaxViewModel(message: "Order detail has been updated.");
            return Json(ajaxViewModel);
        }

        public ActionResult DeleteDetail(Guid[] arrayOfId)
        {
            Array.ForEach(arrayOfId, OrderDetailSessionWrapper.DeleteDetatil);
            var ajaxViewModel = new AjaxViewModel();
            return Json(ajaxViewModel);
        }

        public ActionResult ListOrderDetail([DataSourceRequest] DataSourceRequest request)
        {
            var detailList = OrderDetailSessionWrapper.Detail;
            return Json(detailList.ToDataSourceResult(request));
        }

        public ActionResult History()
        {
            var model = new HistoryViewModel();
            return View(model);
        }

        public ActionResult ListHistory([DataSourceRequest] DataSourceRequest request)
        {
            var historyList = orderProvider.ListOrderHistory();
            return Json(historyList.ToDataSourceResult(request));
        }

        public ActionResult Detail(long id, string mode)
        {
            var model = new DetailViewModel();
            var detailModel = new List<OrderDetailEntryViewModel>();

            Order order = orderProvider.GetOrder(id);
            var orderDetail = orderProvider.GetOrderDetail(id);

            mapper.Map(order, model);
            model.TaxPercent = configurationProvider.GetConfiguration<decimal>(ConfigurationKeys.TaxPercent);
            //model.ServiceChargePercent = configurationProvider.GetConfiguration<decimal>(ConfigurationKeys.ServiceChargePercent);
            model.UnitPriceMode = orderProvider.GetUnitPriceMode(order.UnitPriceMode);
            mapper.Map(orderDetail, detailModel);
            model.List = detailModel;

            ViewBag.mode = mode.XorString();
            if(!IsDetailModeValid(ViewBag.mode))
            {
                return RedirectToAction("Index", "Home", new { area = String.Empty });
            }
            return View(model);
        }

        private bool IsDetailModeValid(string mode)
        {
            return new string[] { "void", "history", "confirmation" }.Contains(mode);
        }

        [HttpPost]
        public ActionResult IsAllowVoidOrder(string userName, string password)
        {
            return Json(new AjaxViewModel(orderProvider.IsAllowVoidOrder(userName, password)));
        }

        [HttpPost]
        public ActionResult Void(long orderId, string userName)
        {
            try
            {
                orderProvider.VoidOrder(orderId, userName);
                return Json(new AjaxViewModel(true));
            }
            catch(Exception ex)
            {
                return Json(new AjaxViewModel(false, message: ex.Message));
            }
        }

        public ActionResult Void(long id)
        {
            ViewBag.VoidAuthenticationUrl = Url.Action("IsAllowVoidOrder");
            ViewBag.VoidOrderId = id;
            ViewBag.VoidUrl = Url.Action("Void");
            return PartialView();
        }
    }
}