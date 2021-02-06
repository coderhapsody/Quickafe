using AutoMapper;
using Microsoft.Owin;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Providers;
using Quickafe.Providers.ViewModels;
using Quickafe.Providers.ViewModels.ProductCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Diagnostics;
using System.Reflection;
using Quickafe.Web.Filters;
using Quickafe.Framework.Attributes;

namespace Quickafe.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly SecurityProvider securityProvider;
        private readonly ConfigurationProvider configurationProvider;
        private readonly IOwinContext owinContext;
        private readonly IMapper mapper;

        public HomeController(CacheDataModel cacheDataModel, SecurityProvider securityProvider, ConfigurationProvider configurationProvider, IMapper mapper, IOwinContext owinContext)
        {
            this.mapper = mapper;
            this.owinContext = owinContext;
            this.securityProvider = securityProvider;
            this.configurationProvider = configurationProvider;
        }

        public ActionResult About()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            ViewBag.VersionNumber = currentAssembly.GetName().Version.ToString(3);
            ViewBag.Copyright = versionInfo.LegalCopyright;
            return PartialView();
        }

        public ActionResult Index()
        {
            var model = new LandingPageViewModel();
            model.StoreName = configurationProvider[ConfigurationKeys.StoreName];
            model.ServerInfo = new Dictionary<string, string>();

            model.ServerInfo.Add("Database Server", configurationProvider.ServerName);
            model.ServerInfo.Add("Database Name", configurationProvider.DatabaseName);
            model.ServerInfo.Add("Application Server", Request.ServerVariables["SERVER_NAME"]);
            model.ServerInfo.Add("Browser", Request.Browser.Browser + " " + Request.Browser.Version);

            return View(model);
        }

        [HttpGet]
        [ImportModelStateFromTempData]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ExportModelStateToTempData]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (securityProvider.ValidateUser(CurrentUserName, model.OldPassword))
                {
                    securityProvider.ChangePassword(CurrentUserName, model.NewPassword);
                    ModelState.Clear();
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Current/old password is invalid");
                    RedirectToAction("ChangePassword");
                }
            }
            return RedirectToAction("ChangePassword");
        }

        public ActionResult ChangePasswordSuccess() => View();

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            owinContext.Authentication.SignOut();        
            return RedirectToAction("Index");
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult GetRootMenus()
        {
            var rootMenus = securityProvider.ListMenus(null);
            return PartialView("_RootMenus", rootMenus);
        }

        public ActionResult GetMenus(int parentMenuId)
        {
            var rootMenus = securityProvider.ListMenus(parentMenuId);
            return PartialView("_Menus", rootMenus);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(System.Web.Mvc.FormCollection form)
        {
            string userName = form["username"];
            string password = form["password"];
            if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
            {
                if (securityProvider.ValidateUser(userName, password))
                {
                    UserLogin userLogin = securityProvider.LogInUser(userName);
                    var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) },
                                    "ApplicationCookie",
                                    ClaimTypes.Name, 
                                    ClaimTypes.Role);

                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, userLogin.Role.Name));
                    claimsIdentity.AddClaim(new Claim(QuickafeClaimTypes.AllowPrintReceipt, userLogin.AllowPrintReceipt.ToString()));
                    claimsIdentity.AddClaim(new Claim(QuickafeClaimTypes.AllowVoidOrder, userLogin.AllowVoidOrder.ToString()));
                    claimsIdentity.AddClaim(new Claim(QuickafeClaimTypes.AllowVoidPayment, userLogin.AllowVoidPayment.ToString()));

                    var authenticationProperties = new AuthenticationProperties { IsPersistent = false };
                    owinContext.Authentication.SignIn(authenticationProperties, claimsIdentity);
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError("Login", "Invalid User Name or Password");
            return View();
        }
    }
}
