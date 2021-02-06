using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quickafe.DataAccess;

namespace Quickafe.Providers
{
    public class LookUpProvider : ConfigurationProvider
    {
        private const string LookUpIdentifier = "LookUp.";

        public LookUpProvider(IQuickafeDbContext context) : base(context)
        {
        }

        private string QualifyLookUpName(string lookUpName) => $"{LookUpIdentifier}{lookUpName}";


        public IEnumerable<string> GetLookUpValues(string lookUpName)
        {
            string configKey = QualifyLookUpName(lookUpName);
            string configValue = this[configKey];
            if (!String.IsNullOrEmpty(configValue))
            {
                string[] lookUpValues = configValue.Split(',');
                foreach (string lookUpValue in lookUpValues)
                    yield return lookUpValue;
            }
        }        

        public IEnumerable<string> GetLookUpNames() =>
            DataContext.Configurations
                        .Where(config => config.Key.StartsWith(LookUpIdentifier))
                        .Select(config => config.Key.Substring(LookUpIdentifier.Length));
        
        public void SaveLookUpValues(string lookUpName, string lookUpValues) =>        
            this[QualifyLookUpName(lookUpName)] = lookUpValues;

    }
}
