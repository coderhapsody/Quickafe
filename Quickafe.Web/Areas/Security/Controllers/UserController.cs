using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Quickafe.DataAccess;
using Quickafe.Providers;
using Quickafe.Providers.ViewModels.User;
using Quickafe.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Areas.Security.Controllers
{
    public class UserController : Controller
    {
        private readonly IMapper mapper;
        private readonly SecurityProvider securityProvider;

        public UserController(SecurityProvider securityProvider, IMapper mapper)
        {
            this.mapper = mapper;
            this.securityProvider = securityProvider;
        }

        public ActionResult Index()
        {
            var model = new IndexViewModel();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new CreateEditViewModel();
            model.IsActive = true;
            return PartialView("Create", model);
        }

        public ActionResult Edit(int id)
        {
            var model = new CreateEditViewModel();
            var userLogin = securityProvider.GetUserLogin(id);
            mapper.Map(userLogin, model);

            return PartialView("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(CreateEditViewModel model)
        {
            var userLogin = securityProvider.GetUserLogin(model.Id);
            bool isSystemUser = userLogin.IsSystemUser;           
            mapper.Map(model, userLogin);
            userLogin.IsSystemUser = isSystemUser;
            if (isSystemUser)
                userLogin.IsActive = true;
            securityProvider.UpdateUserLogin(userLogin);

            var jsonViewModel = new AjaxViewModel(true, model, null);
            return Json(jsonViewModel);
        }

        [HttpPost]
        public ActionResult Create(FormCollection form, CreateEditViewModel model)
        {
            var userLogin = new UserLogin();
            mapper.Map(model, userLogin);
            userLogin.Password = form["password"];
            securityProvider.AddUserLogin(userLogin);

            var jsonViewModel = new AjaxViewModel(true, model, null);
            return Json(jsonViewModel);
        }

        public ActionResult GetRoles()
        {
            var roles = securityProvider.GetRoles().Select(role => new { role.Id, role.Name });
            return Json(roles, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List([DataSourceRequest] DataSourceRequest request)
        {
            var list = securityProvider.ListUserLogins();
            return Json(list.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Delete(int[] arrayOfId)
        {
            try
            {
                Array.ForEach(arrayOfId, securityProvider.DeleteUserLogin);
                return Json(new AjaxViewModel(true, null, null));
            }
            catch (Exception ex)
            {
                return Json(new AjaxViewModel(false, null, ex.Message));
            }
        }
    }
}