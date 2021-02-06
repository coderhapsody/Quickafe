using System.Web.Mvc;

namespace Quickafe.Web.Areas.Master
{
    public class MasterAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Master";

        public override void RegisterArea(AreaRegistrationContext context) =>
            context.MapRoute(
                "Master_default",
                "Master/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
    }
}