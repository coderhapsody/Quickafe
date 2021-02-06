using AutoMapper;
using Castle.Core.Internal;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Providers;
using Quickafe.ViewModels;
using Quickafe.Providers.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quickafe.Framework;

namespace Quickafe.Web.Areas.Master.Controllers
{
    public class ProductController : BaseController
    {
        private readonly ProductProvider productProvider;
        private readonly IMapper mapper;
        private readonly ProductCategoryProvider productCategoryProvider;

        public ProductController(ProductProvider productProvider, ProductCategoryProvider productCategoryProvider, IMapper mapper)
        {
            this.mapper = mapper;
            this.productProvider = productProvider;
            this.productCategoryProvider = productCategoryProvider;
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
            var productCategory = productProvider.GetProduct(id);
            mapper.Map(productCategory, model);

            return PartialView("CreateEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(CreateEditViewModel model)
        {
            var product = new Product();
            mapper.Map(model, product);
            try
            {
                productProvider.UpdateProduct(product);
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
            var product = new Product();
            mapper.Map(model, product);

            try
            {
                productProvider.AddProduct(product);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
            var jsonViewModel = new AjaxViewModel(true, model, null);
            return Json(jsonViewModel);
        }

        public ActionResult List([DataSourceRequest] DataSourceRequest request)
        {
            var list = productProvider.ListProducts(finishedGoods: null);       
            return Json(list.ToDataSourceResult(request));
        }

        public ActionResult GetYieldProducts(int productCategoryId)
        {
            var yieldProducts = productProvider.GetYieldProducts(productCategoryId);       
            return Json(yieldProducts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductCategories()
        {
            var productCategories = productCategoryProvider.GetProductCategories();            
            return JsonNetResult(productCategories);
        }

        [HttpPost]
        public ActionResult Delete(IEnumerable<long> arrayOfId)
        {
            try
            {
                arrayOfId.ForEach(productProvider.DeleteProduct);
                return Json(new AjaxViewModel(true, null, null));
            }
            catch (Exception ex)
            {
                return Json(new AjaxViewModel(false, null, ex.Message));
            }
        }
    }
}