using Quickafe.Framework.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quickafe.DataAccess;
using Quickafe.Providers.ViewModels.Reports;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Quickafe.Providers
{
    public class ReportProvider 
    {
        public IQuickafeDbContext DataContext { get; set; }

        public IEnumerable<OrderReceiptViewModel> OrderReceipt(long orderId)
        {
            var queryReport = from order in DataContext.Orders
                              join orderDetail in DataContext.OrderDetails on order.Id equals orderDetail.OrderId
                              join product in DataContext.Products on orderDetail.ProductId equals product.Id
                              join table in DataContext.Tables on order.TableId equals table.Id
                              where order.Id == orderId
                              select new OrderReceiptViewModel
                              {
                                  OrderNo = order.OrderNo,
                                  OrderDate = order.Date,
                                  ProductCode = product.Code,
                                  ProductName = product.Name,
                                  Qty = orderDetail.Qty,
                                  UnitPrice = orderDetail.UnitPrice,
                                  TableCode = table.Code,
                                  DiscPercent = orderDetail.DiscPercent,
                                  DiscValue = orderDetail.DiscValue
                              };
               
            return queryReport.ToList();
        }

        public IEnumerable<ProductSalesByCategoryViewModel> ProductSalesByCategory(DateTime fromDate, DateTime toDate)
        {
            QuickafeDbContext context = DataContext as QuickafeDbContext;
            var queryReport = context.Database.SqlQuery<ProductSalesByCategoryViewModel>("proc_ReportProductSalesByCategory @FromDate, @ToDate",                 
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate));
            return queryReport.ToList();
        }

        public IEnumerable<PaymentReceiptViewModel> PaymentReceipt(long paymentId)
        {
            var queryReport = from pay in DataContext.Payments
                              join payDetail in DataContext.PaymentDetails on pay.Id equals payDetail.PaymentId
                              join payType in DataContext.PaymentTypes on payDetail.PaymentTypeId equals payType.Id
                              where pay.Id == paymentId
                              select new PaymentReceiptViewModel
                              {
                                  PaymentType = payType.Name,
                                  Amount = payDetail.Amount,                                  
                              };
            return queryReport.ToList();
        }

        public IEnumerable<PaymentByTypeViewModel> PaymentByType(DateTime fromDate, DateTime toDate)
        {
            var queryReport = from pay in DataContext.Payments
                              join payDetail in DataContext.PaymentDetails on pay.Id equals payDetail.PaymentId
                              join payType in DataContext.PaymentTypes on payDetail.PaymentTypeId equals payType.Id
                              where pay.Date >= fromDate && pay.Date <= toDate && !pay.VoidWhen.HasValue
                              group payDetail by payType.Name into g                             
                              select new PaymentByTypeViewModel
                              {
                                  PaymentType = g.Key,
                                  Amount = g.Sum(p => p.Amount)
                              };
            return queryReport.ToList();
        }

        public IEnumerable<OrderSummaryViewModel> OrderSummary(DateTime fromDate, DateTime toDate)
        {
            var reportData = new OrderSummaryViewModel();

            var queryOrderDetail = (from order in DataContext.Orders
                                    join orderDetail in DataContext.OrderDetails on order.Id equals orderDetail.OrderId
                                    where order.Date >= fromDate && order.Date <= toDate && !order.VoidWhen.HasValue
                                    select orderDetail).ToList();

            var queryOrder = (from order in DataContext.Orders
                              where order.Date >= fromDate && order.Date <= toDate && !order.VoidWhen.HasValue
                              select order).ToList();

            var queryPaidOrder = (from order in DataContext.Orders
                                  join payment in DataContext.Payments on order.Id equals payment.OrderId
                                  where !order.VoidWhen.HasValue && !payment.VoidWhen.HasValue &&
                                        order.Date >= fromDate && order.Date <= toDate
                                  select payment).ToList();

            var queryActivePayments = DataContext.Payments.Where(p => !p.VoidWhen.HasValue).Select(p => p.Order);
            var queryUnpaidOrder =
                (from order in DataContext.Orders.Include(o => o.Payments)
                join orderDetail in DataContext.OrderDetails on order.Id equals orderDetail.OrderId
                join table in DataContext.Tables on order.TableId equals table.Id
                where (!order.Payments.Any() || !queryActivePayments.Contains(order)) && !order.VoidWhen.HasValue
                       && order.Date >= fromDate && order.Date <= toDate
                select orderDetail).ToList();

            Expression<Func<OrderDetail, decimal>> sumTotalOrderExpr = o =>
                (o.Qty * o.UnitPrice) - (o.DiscPercent > 0 ? o.DiscPercent / 100 * (o.Qty * o.UnitPrice) : o.DiscValue);

            reportData.GrossSales = queryOrderDetail.Sum(sumTotalOrderExpr.Compile());
            reportData.TaxValue = queryOrder.Sum(o => o.TaxAmount);
            reportData.ServiceCharge = queryOrder.Sum(o => o.ServiceCharge);
            reportData.DeliveryCharges = queryOrder.Sum(o => o.DeliveryCharge);
            reportData.DiscValue = queryOrder.Sum(o => o.DiscValue);
            reportData.NettSales = reportData.GrossSales + reportData.DeliveryCharges + reportData.TaxValue + reportData.ServiceCharge - reportData.DiscValue;
            reportData.TotalPaidOrders = queryPaidOrder.Sum(p => p.BilledAmount);
            reportData.TotalUnpaidOrders = queryUnpaidOrder.Sum(sumTotalOrderExpr.Compile());

            yield return reportData;
        }
    }
}
