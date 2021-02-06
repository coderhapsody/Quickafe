using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using System;
using System.Linq;

namespace Quickafe.Providers
{
    public class ConfigurationProvider : BaseProvider
    {
        public ConfigurationProvider(IQuickafeDbContext context) : base(context)
        {
        }

        public string ServerName => DataContext.Database.Connection.DataSource;
        public string DatabaseName => DataContext.Database.Connection.Database;
        
        private Configuration GetConfigurationObject(string key) =>
            DataContext.Configurations.FirstOrDefault(conf => conf.Key == key);

        public string GetConfiguration(string key)
        {
            Configuration config = GetConfigurationObject(key);
            return config == null ? String.Empty : config.Value;
        }

        public T GetConfiguration<T>(string key)
        {
            try
            {
                string configValue = GetConfiguration(key);
                return (T)Convert.ChangeType(configValue, typeof(T));
            }
            catch
            {
                return (T)Convert.ChangeType(default(T), typeof(T));
            }
        }

        public void UpdateConfiguration<T>(string key, T value)
        {
            var config = GetConfigurationObject(key);
            if (config != null && config.Value != Convert.ToString(value))
            {
                config.Value = Convert.ToString(value);
                config.ChangedWhen = DateTime.Now;
                config.ChangedWho = CurrentUserName;
                DataContext.SaveChanges();
            }
        }

        public string this[string key]
        {
            get { return GetConfiguration(key); }
            set { UpdateConfiguration(key, value); }
        }

        public object GetConfiguration(object reportFolder)
        {
            throw new NotImplementedException();
        }
    }
}
