using Autofac;
using Microsoft.Owin;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Providers;
using Quickafe.Providers.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Quickafe.Web
{
    internal class DataModuleConfig : Module
    {
        private string connectionString;

        public DataModuleConfig(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => HttpContext.Current.GetOwinContext());
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication.User);
            RegisterDataContext(builder);
            RegisterProviders(builder);

            base.Load(builder);
        }

        private void RegisterDataContext(ContainerBuilder builder) =>
                builder.RegisterType<QuickafeDbContext>().As<IQuickafeDbContext>()
                .WithParameter("connectionString", connectionString)
                .InstancePerRequest();

        private void RegisterProviders(ContainerBuilder builder) =>
            ThisAssembly.GetReferencedAssemblies()
            .Where(a => a.Name.Contains("Providers"))
            .ToList()
            .ForEach(assemblyName =>
            {
                System.Reflection.Assembly.Load(assemblyName).GetTypes()
                    .Where(a => a.Name.EndsWith("Provider"))
                    .ToList()
                    .ForEach(providerType => builder.RegisterType(providerType)
                                                    .AsSelf()
                                                    .PropertiesAutowired()
                                                    .InstancePerRequest());
            });
    }

}
