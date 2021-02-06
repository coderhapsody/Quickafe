using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.ViewModels.Dashboard
{
    public class MtdOrderVolumeViewModel
    {
        public int Month { get; set; }
        public string MonthName { get; set; } 
        public decimal Volume { get; set; }

    }
}
