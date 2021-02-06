using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.Sales.ViewModels.Payment
{
    public class PaymentDetailViewModel
    {
        public Guid Uid { get; set; }

        [Display(Name = "Payment Type")]
        public long PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Display(Name = "Card No.")]
        public string CardNo { get; set; }
        public string Notes { get; set; }
    }
}
