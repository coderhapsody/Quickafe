using Quickafe.Framework.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.Sales.ViewModels.Order
{
    public class OrderDetailEntryViewModel : BaseViewModel
    {
        public Guid Uid { get; set; }
        public long ProductId { get; set; }

        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        public string ProductCategoryName { get; set; }
        public short Qty { get; set; }

        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Disc. %")]
        public decimal DetailDiscPercent { get; set; }

        [Display(Name = "Disc. Value")]
        public decimal DetailDiscValue { get; set; }

        public decimal Total => Qty * UnitPrice - (DetailDiscPercent > 0 ? DetailDiscPercent / 100*Qty*UnitPrice : DetailDiscValue);

        public string Notes { get; set; }
    }
}
