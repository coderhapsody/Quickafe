using AutoMapper;
using Castle.Core.Internal;
using Quickafe.Providers.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quickafe.Providers.Inventory.ViewModels.InventoryOut;
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
    public class InventoryOutController : BaseController
    {
        private readonly LookUpProvider lookUpProvider;
        private readonly InventoryOutProvider inventoryOutProvider;
        private readonly IMapper mapper;
        private readonly AutoNumberProvider autoNumberProvider;
        public int cntId = 0;

        public InventoryOutController(LookUpProvider lookUpProvider, InventoryOutProvider inventoryOutProvider, IMapper mapper, AutoNumberProvider autoNumberProvider)
        {
            this.lookUpProvider = lookUpProvider;
            this.inventoryOutProvider = inventoryOutProvider;
            this.mapper = mapper;
            this.autoNumberProvider = autoNumberProvider;
        }

        // GET: Inventory/InventoryOut
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
                inventoryOutProvider.PostOutInventory(arrayOfId);
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
                return Json(new AjaxViewModel(false, ex, ex.Message));
            }
            return Json(new AjaxViewModel(true));
        }

        public ActionResult ListInventoryOut([DataSourceRequest] DataSourceRequest request)
        {
            var inventoryList = inventoryOutProvider.GetInventory();
            return Json(inventoryList.ToDataSourceResult(request));
        }

        // GET: Inventory/WarehouseIN/Create
        public ActionResult Create()
        {
            var model = new CreateEditViewModel();
            model.InventoryNo = "NEW";
            model.InventoryDate = System.DateTime.Today;
            model.Direction = "O";
            InventoryOutDetailSessionWrapper.Initialize();

            return View("CreateEdit", model);
        }

        [HttpPost]
        public ActionResult ValidateInventoryOut(CreateEditViewModel model, FormCollection form)
        {
            AjaxViewModel validationResult;
            if (InventoryOutDetailSessionWrapper.IsEmpty)
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
            var inventoryOutDetails = new List<Quickafe.DataAccess.InventoryDetail>();
            model.Id = 0;
            model.LocationId = inventoryOutProvider.GetLocationId();
            mapper.Map(model, inventory);
            mapper.Map(InventoryOutDetailSessionWrapper.Detail, inventoryOutDetails);

            inventory.InventoryDetails.AddRange(inventoryOutDetails);
            inventoryOutProvider.AddInventory(inventory);

            return RedirectToAction("Index");
        }

        public ActionResult GetInventoryOutDetail([DataSourceRequest] DataSourceRequest request, long inventoryId)
        {
            var detailList = InventoryOutDetailSessionWrapper.Detail;
            return Json(detailList.ToDataSourceResult(request));
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
                    return new[] { "DAMAGED", "PROCESS", "ADJUSTMENT" };
                case "F": // finish goods
                    return new[] { "DAMAGED", "ADJUSTMENT" };
            }

            return null;
        }


        public ActionResult CreateDetail()
        {
            var model = new InventoryDetailEntryViewModel();
            return PartialView("CreateEditDetail", model);
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
                model.UomId = inventoryOutProvider.GetUOMId();
                InventoryOutDetailSessionWrapper.AddDetail(model);
                //var summary = OrderDetailSessionWrapper.CalculateSummary();
                var ajaxViewModel = new AjaxViewModel(message: "Inventory detail has been added.");
                return Json(ajaxViewModel);
            }
        }


        [HttpGet]
        public ActionResult EditDetail(int id)
        {
            //Guid detailUid = Guid.Parse(detailUidString);
            var model = InventoryOutDetailSessionWrapper.GetDetail(id);
            return PartialView("CreateEditDetail", model);
        }

        [HttpPost]
        public ActionResult EditDetail(InventoryDetailEntryViewModel model, FormCollection form)
        {
            InventoryOutDetailSessionWrapper.UpdateDetail(model);
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
                Array.ForEach(arrayOfId, InventoryOutDetailSessionWrapper.DeleteDetail);
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
            var inventory = inventoryOutProvider.GetInventory(id);

            if (inventory != null)
            {
                var inventoryDetail = inventoryOutProvider.GetInventoryDetail(inventory.Id);
                if (inventoryDetail != null)
                {
                    mapper.Map(inventory, inventoryViewModel);
                    mapper.Map(inventoryDetail, inventoryDetailViewModel);
                    InventoryOutDetailSessionWrapper.Initialize(inventoryDetailViewModel);
                    return View("CreateEdit", inventoryViewModel);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(CreateEditViewModel model, FormCollection form)
        {
            var inventory = inventoryOutProvider.GetInventory(model.Id);
            var inventoryDetails = new List<InventoryDetail>();
            mapper.Map(model, inventory);
            mapper.Map(InventoryOutDetailSessionWrapper.Detail, inventoryDetails);
            inventory.InventoryDetails.Clear();
            foreach (InventoryDetail inventoryDetail in inventoryDetails)
            {
                inventoryDetail.InventoryId = inventory.Id;
                inventory.InventoryDetails.Add(inventoryDetail);
            }

            inventoryOutProvider.UpdateInventory(inventory);
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
                arrayOfId.ForEach(inventoryOutProvider.DeleteInventory);
                return Json(new AjaxViewModel(true, null, null));
            }
            catch
            {
                return Json(new AjaxViewModel(false, null, null));
            }
        }

    }
}