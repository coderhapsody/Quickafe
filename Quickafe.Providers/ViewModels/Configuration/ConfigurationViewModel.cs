using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.ViewModels.Configuration
{
    public class ConfigurationViewModel
    {
        public int InvoiceNumberingLength { get; set; }

        [Required]
        public string InvoicePrefix { get; set; }
        public int OrderNumberingLength { get; set; }

        [Required]
        public string OrderPrefix { get; set; } 
        public int PaymentNumberingLength { get; set; }

        [Required]
        public string PaymentPrefix { get; set; }
        public decimal ServiceChargePercent { get; set; }

        [Required]
        public string StoreName { get; set; }

        public decimal TaxPercent { get; set; }

        [Required]
        public string UnitPriceLabel { get; set; }
        public string UnitPrice2Label { get; set; }
        public string UnitPrice3Label { get; set; }

    }
}
