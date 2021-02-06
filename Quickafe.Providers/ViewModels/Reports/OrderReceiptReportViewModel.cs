using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.ViewModels.Reports
{
    public class OrderReceiptViewModel
    {
        public string OrderNo { get; set; }
        public string TableCode { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductCode { get; set; }

        public string ProductName { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscPercent { get; set; }
        public decimal DiscValue { get; set; }
    }
}
