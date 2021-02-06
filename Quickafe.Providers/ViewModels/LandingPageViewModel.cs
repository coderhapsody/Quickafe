using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.ViewModels
{
    public class LandingPageViewModel
    {
        public string StoreName { get; set; }

        public Dictionary<string, string> ServerInfo { get; set; }

        public IDictionary<int, decimal> MtdOrderVolume;
    }
}
