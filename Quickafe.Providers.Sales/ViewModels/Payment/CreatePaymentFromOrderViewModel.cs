using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.Sales.ViewModels.Payment
{
    public class CreatePaymentViewModel
    {
        // Billable order id
        public long[] BillableOrders { get; set; }

        [Display(Name = "Service Charge")]
        public decimal ServiceChargeValue { get; set; }

        [Display(Name = "Tax")]
        public decimal TaxValue { get; set; }

        [Display(Name = "Billable Amount")]
        public decimal BillableAmount => (TotalOrders + ServiceChargeValue + TaxValue + DeliveryCharge) - DiscValue;

        [Display(Name = "Delivery Charge")]
        public decimal DeliveryCharge { get; set; }

        [Display(Name = "Discount")]
        public decimal DiscPercent { get; set; }
        public decimal DiscValue { get; set; }

        // For Kendo grid server side binding
        public IEnumerable<PayableOrderViewModel> OrderDetails { get; set; } = new List<PayableOrderViewModel>();

        [Display(Name = "Total Order")]
        public decimal TotalOrders => OrderDetails.Any() ? OrderDetails.Sum(o => o.Quantity * o.UnitPrice - o.DiscValue) : 0;

        // For Kendo grid modelling only
        public IEnumerable<PaymentDetailViewModel> PaymentDetails { get; set; } = new List<PaymentDetailViewModel>();

    }
}

