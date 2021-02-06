using Quickafe.Framework.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.Sales.ViewModels.Order
{
    public class DetailViewModel : ListViewModel<OrderDetailEntryViewModel>
    {
        public long Id { get; set; }

        [Display(Name = "Order No.")]
        public string OrderNo { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Total Order")]
        public decimal TotalOrder => List.Sum(o => o.Qty * o.UnitPrice - (o.DetailDiscPercent > 0 ? o.DetailDiscPercent / 100 * o.Qty * o.UnitPrice : o.DetailDiscValue));
        public string Notes { get; set; }

        [Display(Name = "Order Type")]
        public string OrderType { get; set; }

        [Display(Name = "Table")]
        public string TableCode { get; set; }
        public int Guests { get; set; }

        [Display(Name = "Delivery Charge")]
        public decimal DeliveryCharge { get; set; }

        [Display(Name = "Tax")]
        public decimal TaxPercent { get; set; }
        public decimal TaxValue => TaxPercent / 100 * (TotalOrder - DiscValue);

        //[Display(Name = "Service Charge")]
        //public decimal ServiceChargePercent { get; set; }
        //public decimal ServiceChargeValue => ServiceChargePercent / 100 * TotalOrder;

        [Display(Name = "Discount")]
        public decimal DiscPercent { get; set; }
        public decimal DiscValue { get; set; }

        [Display(Name = "Billable Amount")]
        public decimal BillableAmount => TotalOrder + DeliveryCharge - DiscValue + TaxValue /*+ ServiceChargeValue*/;

        [Display(Name = "Pricing Mode")]
        public string UnitPriceMode { get; set; }
    }
}
