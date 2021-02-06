﻿using System.Web.Mvc;

namespace Quickafe.Web.Areas.Sales
{
    public class SalesAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Sales";

        public override void RegisterArea(AreaRegistrationContext context) =>        
            context.MapRoute(
                "Sales_default",
                "Sales/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
    }
}