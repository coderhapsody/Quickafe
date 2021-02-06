using AutoMapper;
using Quickafe.Framework.Base;
using Quickafe.Providers;
using Quickafe.Providers.ViewModels.RoleAccess;
using Quickafe.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web.Areas.Security.Controllers
{
    public class RoleAccessController : BaseController
    {
        private readonly SecurityProvider securityProvider;
        private readonly IMapper mapper;

        public RoleAccessController(SecurityProvider securityProvider, IMapper mapper)
        {
            this.securityProvider = securityProvider;
            this.mapper = mapper;
        }
        
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListMenus(int? id)
        {
            var menus = securityProvider.ListAllMenus(id);
            return Json(menus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRolesOfMenu(long menuId)
        {
            var roles = securityProvider.GetRolesOfMenu(menuId);
            return JsonNetResult(roles);//, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(FormCollection form, IList<RoleAccessViewModel> model)
        {
            if (model.Count > 0)
            {
                long menuId = model[0].MenuId;
                //IList<RoleMenu> roleMenus = new List<RoleMenu>();
                //roleMenus = mapper.Map<IList<RoleAccessViewModel>, IList<RoleMenu>>(model.Where(m => m.RoleId != 0).ToList());
                securityProvider.UpdateRoleAccess(menuId, model.Where(m => m.IsAllowed).Select(m => m.RoleId));
            }
            var ajaxViewModel = new AjaxViewModel(true, model, null);
            return Json(ajaxViewModel);
        }

    }
}