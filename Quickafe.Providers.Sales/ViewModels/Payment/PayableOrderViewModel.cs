using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.Sales.ViewModels.Payment
{
    public class PayableOrderViewModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal DiscPercent { get; set; }
        public decimal DiscValue { get; set; }
        public decimal Total => Quantity * UnitPrice - DiscValue;
    }
}
