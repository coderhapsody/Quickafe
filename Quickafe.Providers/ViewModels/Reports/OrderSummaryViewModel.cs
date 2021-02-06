using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.ViewModels.Reports
{
    public class OrderSummaryViewModel
    {
        public decimal GrossSales { get; set; }

        public decimal DeliveryCharges { get; set; }

        public decimal TaxValue { get; set; }

        public decimal ServiceCharge { get; set; }

        public decimal DiscValue { get; set; }

        public decimal NettSales { get; set; }

        public decimal TotalUnpaidOrders { get; set; }

        public decimal TotalPaidOrders { get; set; }
    }
}
