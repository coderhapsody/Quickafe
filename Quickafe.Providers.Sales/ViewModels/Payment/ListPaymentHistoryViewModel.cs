using Quickafe.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.Sales.ViewModels.Payment
{
    public class ListPaymentHistoryViewModel
    {
        public long Id { get; set; }
        public string PaymentNo { get; set; }
        public DateTime PaymentDate { get; set; }
        public long OrderId { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; }
        public decimal BillableAmount { get; set; }
        public decimal PaidAmount { get; set; }

        public string PaymentTypes { get; set; }
    }
}
