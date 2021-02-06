using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.Sales.ViewModels.Order
{
    public class ListOrderHistoryViewModel : DataAccess.Order
    {
        public string TableCode { get; set; }
        public string PaymentNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal TotalOrder { get; set; }
        public decimal? TotalPayment { get; set; }
        public bool IsOrderVoid { get; set; }
    }
}
