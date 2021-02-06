using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers
{
    public static class ConfigurationKeys
    {
        public static readonly string InvoiceNumberingLength = nameof(InvoiceNumberingLength);
        public static readonly string InvoicePrefix = nameof(InvoicePrefix);
        public static readonly string OrderNumberingLength = nameof(OrderNumberingLength);
        public static readonly string OrderPrefix = nameof(OrderPrefix);
        public static readonly string PaymentNumberingLength = nameof(PaymentNumberingLength);
        public static readonly string PaymentPrefix = nameof(PaymentPrefix);
        public static readonly string ServiceChargePercent = nameof(ServiceChargePercent);
        public static readonly string StoreName = nameof(StoreName);
        public static readonly string TaxPercent = nameof(TaxPercent);
        public static readonly string ReportServerUrl = nameof(ReportServerUrl);
        public static readonly string ReportFolder = nameof(ReportFolder);
        public static readonly string UnitPriceMode1 = nameof(UnitPriceMode1);
        public static readonly string UnitPriceMode2 = nameof(UnitPriceMode2);
        public static readonly string UnitPriceMode3 = nameof(UnitPriceMode3);
    }
}
