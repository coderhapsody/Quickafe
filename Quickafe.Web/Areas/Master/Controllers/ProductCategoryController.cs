using AutoMapper;
using Castle.Core.Internal;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Providers;
using Quickafe.ViewModels;
using Quickafe.Providers.ViewModels.ProductCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Areas.Master.Controllers
{
    public class ProductCategoryController : BaseController
    {
        private ProductCategoryProvider productCategoryProvider;
        private readonly IMapper mapper;

        public ProductCategoryController(ProductCategoryProvider productCategoryProvider, IMapper mapper)
        {
            this.mapper = mapper;
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
            var productCategory = productCategoryProvider.GetProductCategory(id);
            mapper.Map(productCategory, model);

            return PartialView("CreateEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(CreateEditViewModel model)
        {
            var productCategory = new ProductCategory();
            mapper.Map(model, productCategory);
            try
            {
                productCategoryProvider.UpdateProductCategory(productCategory);
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
            var productCategory = new ProductCategory();
            mapper.Map(model, productCategory);
            try
            {
                productCategoryProvider.AddProductCategory(productCategory);
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
            var list = productCategoryProvider.GetProductCategories();
            return Json(list.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Delete(IEnumerable<long> arrayOfId)
        {
            try
            {
                arrayOfId.ForEach(productCategoryProvider.DeleteProductCategory);
                return Json(new AjaxViewModel(true, null, null));
            }
            catch (Exception ex)
            {
                return Json(new AjaxViewModel(false, null, ex.Message));
            }
        }
    }
}