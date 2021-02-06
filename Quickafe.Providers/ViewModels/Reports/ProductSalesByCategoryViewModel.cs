using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.ViewModels.Reports
{
    public class ProductSalesByCategoryViewModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public int Qty { get; set; }        
        public decimal GrossSales { get; set; }
        public decimal TotalDiscounts { get; set; }
        public decimal NettSales { get; set; }
    }
}
