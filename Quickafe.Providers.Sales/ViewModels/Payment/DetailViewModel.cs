using Quickafe.Framework.Base;
using Quickafe.Providers.Sales.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.Sales.ViewModels.Payment
{
    public class DetailViewModel : ListViewModel<PaymentDetailViewModel>
    {
        public int Id { get; set; }

        [Display(Name = "Payment No.")]
        public string PaymentNo { get; set; }

        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        public long OrderId { get; set; }

        [Display(Name = "Order No.")]
        public string OrderNo { get; set; }

        [Display(Name = "Billable Amount")]
        public decimal BillableAmount => TotalOrder + DeliveryCharge + TaxValue /* + ServiceCharge*/ - DiscValue;

        [Display(Name = "Paid Amount")]
        public decimal PaidAmount { get; set; }

        [Display(Name = "Order Type")]
        public string OrderType { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Total Payment")]
        public decimal TotalPayment => List.Sum(o => o.Amount);

        [Display(Name = "Delivery Charge")]
        public decimal DeliveryCharge { get; set; }

        //[Display(Name = "Service Charge")]
        //public decimal ServiceCharge { get; set; }

        [Display(Name = "Tax")]
        public decimal TaxValue { get; set; }

        [Display(Name = "Discount")]
        public decimal DiscValue { get; set; }

        [Display(Name = "Total Order")]
        public decimal TotalOrder => OrderDetails.Sum(o => o.Qty * o.UnitPrice - (o.DetailDiscPercent > 0 ? o.DetailDiscPercent / 100 * o.Qty * o.UnitPrice : o.DetailDiscValue));

        public IEnumerable<OrderDetailEntryViewModel> OrderDetails { get; set; }

        [Display(Name = "Pricing Mode")]
        public string UnitPriceMode { get; set; }
    }
}
