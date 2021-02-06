using Autofac;
using log4net;

namespace Quickafe.Web
{
    public class LogModuleConfig : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            log4net.Config.XmlConfigurator.Configure();
            builder.Register(c => LogManager.GetLogger(this.GetType())).PropertiesAutowired();
            base.Load(builder);
        }
    }
}