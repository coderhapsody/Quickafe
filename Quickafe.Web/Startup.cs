using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Web;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(Quickafe.Web.Startup))]
namespace Quickafe.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AutofacConfig.ConfigureContainer();
            ConfigureAuthentication(app);
        }

        public void ConfigureAuthentication(IAppBuilder app)
        {
            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",

                LoginPath = new PathString("/Home/Login"),
                ExpireTimeSpan = TimeSpan.FromMinutes(60),
                CookieName = "Quickafe"
            });
        }
    }
}