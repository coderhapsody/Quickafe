using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Quickafe.DataAccess;
using Quickafe.Providers;
using Quickafe.Providers.ViewModels.Role;
using Quickafe.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Areas.Security.Controllers
{
    public class RoleController : Controller
    {
        private readonly SecurityProvider securityProvider;
        private readonly IMapper mapper;

        public RoleController(SecurityProvider securityProvider, IMapper mapper)
        {
            this.securityProvider = securityProvider;
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

        public ActionResult Edit(int id)
        {
            var model = new CreateEditViewModel();
            var role = securityProvider.GetRole(id);
            mapper.Map(role, model);

            return PartialView("CreateEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(CreateEditViewModel model)
        {
            var role = securityProvider.GetRole(model.Id);
            mapper.Map(model, role);
            securityProvider.UpdateRole(role);

            var jsonViewModel = new AjaxViewModel(true, role, null);
            return Json(jsonViewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateEditViewModel model)
        {
            var role = new Role();
            mapper.Map(model, role);
            securityProvider.AddRole(role);

            var jsonViewModel = new AjaxViewModel(true, role, null);
            return Json(jsonViewModel);
        }

        public ActionResult List([DataSourceRequest] DataSourceRequest request)
        {
            var list = securityProvider.List();
            return Json(list.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Delete(long[] arrayOfId)
        {
            try
            {
                Array.ForEach(arrayOfId, securityProvider.DeleteRole);
                return Json(new AjaxViewModel(true, null, null));
            }
            catch (Exception ex)
            {
                return Json(new AjaxViewModel(false, null, ex.Message));
            }
        }
    }
}