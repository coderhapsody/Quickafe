using Autofac;
using Autofac.Integration.Mvc;
using Quickafe.Web.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web
{
    internal static class AutofacConfig
    {
        internal static readonly string DefaultConnectionName = "DefaultConnection";
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new LogModuleConfig());

            builder.RegisterType<ReportPreview>().AsSelf().PropertiesAutowired();

            // Register dependencies in controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();

            // Register dependencies in filter attributes
            builder.RegisterFilterProvider();

            // Register dependencies in custom views
            builder.RegisterSource(new ViewRegistrationSource());

            // Register our Data dependencies
            builder.RegisterModule(new DataModuleConfig(DefaultConnectionName));

            builder.RegisterModule(new MapperConfig());

            builder.RegisterModule(new CacheConfig(DefaultConnectionName));

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}