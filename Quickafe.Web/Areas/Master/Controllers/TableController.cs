using AutoMapper;
using Castle.Core.Internal;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Providers;
using Quickafe.Providers.ViewModels.Table;
using Quickafe.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Areas.Master.Controllers
{
    public class TableController : BaseController
    {
        private readonly TableProvider tableProvider;
        private readonly IMapper mapper;

        public TableController(TableProvider tableProvider, IMapper mapper)
        {
            this.tableProvider = tableProvider;
            this.mapper = mapper;
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
            var table = tableProvider.GetTable(id);
            mapper.Map(table, model);

            return PartialView("CreateEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(CreateEditViewModel model)
        {
            var table = new Table();
            mapper.Map(model, table);
            try
            {
                tableProvider.UpdateTable(table);
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
            var table = new Table();
            mapper.Map(model, table);
            try
            {
                tableProvider.AddTable(table);
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
            var list = tableProvider.GetTables();
            return Json(list.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Delete(IEnumerable<long> arrayOfId)
        {
            try
            {
                arrayOfId.ForEach(tableProvider.DeleteTable);
                return Json(new AjaxViewModel(true, null, null));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}