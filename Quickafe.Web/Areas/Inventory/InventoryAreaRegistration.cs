using System.Web.Mvc;

namespace Quickafe.Web.Areas.Inventory
{
    public class InventoryAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Inventory";

        public override void RegisterArea(AreaRegistrationContext context) =>
            context.MapRoute(
                "Inventory_default",
                "Inventory/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
    }
}