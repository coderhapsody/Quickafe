using AutoMapper;
using Castle.Core.Internal;
using Quickafe.Providers.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quickafe.Providers.Inventory.ViewModels.InventoryIn;
using Quickafe.Providers;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ProductViewModels = Quickafe.Providers.ViewModels.Product;
using Quickafe.ViewModels;
using Quickafe.Web.SessionWrappers;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;

namespace Quickafe.Web.Areas.Inventory.Controllers
{
    public class InventoryInController : BaseController
    {

        private readonly LookUpProvider lookUpProvider;
        private readonly InventoryInProvider inventoryInProvider;
        private readonly IMapper mapper;
        private readonly AutoNumberProvider autoNumberProvider;
        public int cntId = 0;

        public InventoryInController(LookUpProvider lookUpProvider, InventoryInProvider inventoryInProvider, IMapper mapper, AutoNumberProvider autoNumberProvider)
        {
            this.lookUpProvider = lookUpProvider;
            this.inventoryInProvider = inventoryInProvider;
            this.mapper = mapper;
            this.autoNumberProvider = autoNumberProvider;
        }

        // GET: Inventory/WarehouseIN
        public ActionResult Index()
        {
            var model = new IndexViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Posting(long[] arrayOfId)
        {
            try
            {
                inventoryInProvider.PostInInventory(arrayOfId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(new AjaxViewModel(false, ex, ex.Message));
            }
            return Json(new AjaxViewModel(true));
        }

        public ActionResult ListInventoryIn([DataSourceRequest] DataSourceRequest request)
        {
            var inventoryList = inventoryInProvider.GetInventory();
            return Json(inventoryList.ToDataSourceResult(request));
        }


        // GET: Inventory/WarehouseIN/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Inventory/WarehouseIN/Create
        public ActionResult Create()
        {
            var model = new CreateEditViewModel();
            model.InventoryNo = "NEW";
            model.InventoryDate = System.DateTime.Today;
            model.Direction = "I";
            InventoryInDetailSessionWrapper.Initialize();

            return View("CreateEdit", model);
        }

        [HttpPost]
        public ActionResult ValidateInventoryIn(CreateEditViewModel model, FormCollection form)
        {
            AjaxViewModel validationResult;
            if (InventoryInDetailSessionWrapper.IsEmpty)
            {
                validationResult = new AjaxViewModel(isSuccess: false, message: "Inventory Detail is Empty");
                return Json(validationResult);
            }
            validationResult = new AjaxViewModel(isSuccess: true);
            return Json(validationResult);
        }

        // POST: Inventory/WarehouseIN/Create
        [HttpPost]
        public ActionResult Create(CreateEditViewModel model, FormCollection form)
        {
            var inventory = new Quickafe.DataAccess.Inventory();
            var inventoryInDetails = new List<Quickafe.DataAccess.InventoryDetail>();
            model.Id = 0;
            model.LocationId = inventoryInProvider.GetLocationId();

            mapper.Map(model, inventory);
            mapper.Map(InventoryInDetailSessionWrapper.Detail, inventoryInDetails);

            inventory.InventoryDetails.AddRange(inventoryInDetails);
            inventoryInProvider.AddInventory(inventory);
            //return RedirectToAction("OrderConfirmation", new { inventoryId = inventory.Id });
            return RedirectToAction("Index");
        }

        public ActionResult GetInventoryInDetail([DataSourceRequest] DataSourceRequest request, long inventoryId)
        {
            var detailList = InventoryInDetailSessionWrapper.Detail;
            return Json(detailList.ToDataSourceResult(request));
        }


        public ActionResult CreateDetail()
        {
            var model = new InventoryDetailEntryViewModel();
            return PartialView("CreateEditDetail", model);
        }

        [HttpPost]
        public ActionResult ValidateInventoryDetailIn(InventoryDetailEntryViewModel model, FormCollection form)
        {
            AjaxViewModel validationResult;
            if (model.ProductId == 0 || model.Qty == 0)
            {
                validationResult = new AjaxViewModel(isSuccess: false, message: "Inventory Detail is Not Complete");
                return Json(validationResult);
            }
            else
            {
                validationResult = new AjaxViewModel(isSuccess: true);
                return Json(validationResult);
            }
        }

        [HttpPost]
        public ActionResult CreateDetail(InventoryDetailEntryViewModel model, FormCollection form)
        {
            if (model.ProductId == 0)
            {
                var ajaxViewModel = new AjaxViewModel(isSuccess: false, data: null, message: "Please Choose Product!");
                return Json(ajaxViewModel);
            }
            else if (model.Qty == 0)
            {
                var ajaxViewModel = new AjaxViewModel(isSuccess: false, data: null, message: "Please Fill Qty!");
                return Json(ajaxViewModel);

            }
            else
            {
                // Get UOM
                model.UomId = inventoryInProvider.GetUOMId();
                InventoryInDetailSessionWrapper.AddDetail(model);
                //var summary = OrderDetailSessionWrapper.CalculateSummary();
                var ajaxViewModel = new AjaxViewModel(message: "Inventory detail has been added.");
                return Json(ajaxViewModel);
            }
        }


        [HttpGet]
        public ActionResult EditDetail(int id)
        {
            //Guid detailUid = Guid.Parse(detailUidString);
            var model = InventoryInDetailSessionWrapper.GetDetail(id);
            return PartialView("CreateEditDetail", model);
        }

        [HttpPost]
        public ActionResult EditDetail(InventoryDetailEntryViewModel model, FormCollection form)
        {
            InventoryInDetailSessionWrapper.UpdateDetail(model);
            //var summary = OrderDetailSessionWrapper.CalculateSummary();
            var ajaxViewModel = new AjaxViewModel(message: "Order detail has been updated.");
            return Json(ajaxViewModel);
        }

        public ActionResult DeleteDetail(long[] arrayOfId)
        {
            if (arrayOfId == null || arrayOfId.Length == 0)
            {
                var ajaxViewModel = new AjaxViewModel(isSuccess: false, data: null, message: "Please Choose Data to Delete");
                return Json(ajaxViewModel);
            }
            else
            {
                Array.ForEach(arrayOfId, InventoryInDetailSessionWrapper.DeleteDetail);
                //var summary = OrderDetailSessionWrapper.CalculateSummary();
                var ajaxViewModel = new AjaxViewModel(isSuccess: true);
                return Json(ajaxViewModel);
            }
        }

        public ActionResult BrowseProduct()
        {
            var productList = new List<ProductViewModels.ListProductViewModel>();
            return PartialView("BrowseProduct", productList);
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var inventoryViewModel = new CreateEditViewModel();
            var inventoryDetailViewModel = new List<InventoryDetailEntryViewModel>();
            var inventory = inventoryInProvider.GetInventory(id);

            if (inventory != null)
            {
                var inventoryDetail = inventoryInProvider.GetInventoryDetail(inventory.Id);
                if (inventoryDetail != null)
                {
                    mapper.Map(inventory, inventoryViewModel);
                    mapper.Map(inventoryDetail, inventoryDetailViewModel);
                    InventoryInDetailSessionWrapper.Initialize(inventoryDetailViewModel);
                    return View("CreateEdit", inventoryViewModel);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(CreateEditViewModel model, FormCollection form)
        {
            var inventory = inventoryInProvider.GetInventory(model.Id);
            var inventoryDetails = new List<InventoryDetail>();
            mapper.Map(model, inventory);
            mapper.Map(InventoryInDetailSessionWrapper.Detail, inventoryDetails);
            inventory.InventoryDetails.Clear();
            foreach (InventoryDetail inventoryDetail in inventoryDetails)
            {
                inventoryDetail.InventoryId = inventory.Id;
                inventory.InventoryDetails.Add(inventoryDetail);
            }

            inventoryInProvider.UpdateInventory(inventory);
            //return RedirectToAction("OrderConfirmation", new { orderId = model.Id });
            return RedirectToAction("Index");
        }

        // POST: Inventory/WarehouseIN/Delete/5
        [HttpPost]
        public ActionResult Delete(IEnumerable<long> arrayOfId)
        {
            try
            {
                // TODO: Add delete logic here
                arrayOfId.ForEach(inventoryInProvider.DeleteInventory);
                return Json(new AjaxViewModel(true, null, null));
            }
            catch
            {
                return Json(new AjaxViewModel(false, null, null));
            }
        }

        public ActionResult GetMutationTypes(string inventoryType)
        {
            var mutationTypes = GetMutationPaths(inventoryType);
            return Json(mutationTypes, JsonRequestBehavior.AllowGet);
        }

        private string[] GetMutationPaths(string inventoryType)
        {
            switch (inventoryType)
            {
                case "S": // semi finish goods
                    return new[] { "ADJUSTMENT" };
                case "F": // finish goods
                    return new[] { "PROCESS", "ADJUSTMENT" };
            }

            return null;
        }
    }
}
