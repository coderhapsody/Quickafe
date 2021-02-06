using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quickafe.Providers.Sales.ViewModels.Order
{
    public class CreateEditViewModel : BaseViewModel
    {
        public long Id { get; set; }
        public long LocationId { get; set; }

        [Display(Name = "Order No.")]
        public string OrderNo { get; set; }

        [Display(Name = "Order Type")]
        public string OrderType { get; set; }

        [Display(Name = "Table")]
        public long TableId { get; set; }
        public Table Table { get; set; }
        public int Guests { get; set; }

        [Display(Name = "Deliv. Charge")]
        public decimal DeliveryCharge { get; set; }

        [Display(Name = "Discount")]
        public decimal DiscPercent { get; set; }
        public decimal DiscValue { get; set; }

        [Display(Name = "Order Date")]
        public System.DateTime Date { get; set; }
        public System.DateTime? Start { get; set; }
        public string Notes { get; set; }
        public DateTime? VoidWhen { get; set; }
        public string VoidReason { get; set; }
        public string VoidAuth { get; set; }
        public IEnumerable<OrderDetailEntryViewModel> OrderDetails { get; set; }

        [Display(Name = "Pricing Mode")]
        public int UnitPriceMode { get; set; }
    }
}
