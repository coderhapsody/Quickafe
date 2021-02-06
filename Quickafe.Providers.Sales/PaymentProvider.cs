using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Providers.Sales.ViewModels.Payment;
using Quickafe.Providers.Inventory;

namespace Quickafe.Providers
{
    public class PaymentProvider : BaseProvider
    {
        private readonly AutoNumberProvider autoNumberProvider;
        private readonly ConfigurationProvider configurationProvider;
        private readonly InventoryOutProvider inventoryOutProvider;

        public PaymentProvider(
            IQuickafeDbContext context,
            AutoNumberProvider autoNumberProvider,
            InventoryOutProvider inventoryOutProvider,
            ConfigurationProvider configurationProvider) : base(context)
        {
            this.autoNumberProvider = autoNumberProvider;
            this.inventoryOutProvider = inventoryOutProvider;
            this.configurationProvider = configurationProvider;
        }

        public void AddPayment(Order order, Payment payment)
        {
            object o = new object();
            DataContext.Payments.Add(payment);
            lock (o)
            {
                long lastNumber = autoNumberProvider.GetLastNumber("PAYMENT", payment.Date.Year);
                string prefix = configurationProvider.GetConfiguration<string>(ConfigurationKeys.PaymentPrefix);
                int length = configurationProvider.GetConfiguration<int>(ConfigurationKeys.PaymentNumberingLength);
                lastNumber++;
                string paymentNo = String.Format("{0}{1}", prefix, lastNumber.ToString(String.Concat(Enumerable.Repeat("0", length))));
                payment.PaymentNo = paymentNo;
                autoNumberProvider.IncrementLastNumber("PAYMENT", payment.Date.Year);
                SetAuditFields(payment);
                SetAuditFields(order);

                inventoryOutProvider.PostFromSales(payment);
            }
            DataContext.SaveChanges();
        }

        //public void UpdatePayment(Payment payment)
        //{
        //    DataContext.Payments.Attach(payment);
        //    DataContext.Entry(payment).State = EntityState.Modified;
        //    SetAuditFields(payment);
        //    DataContext.SaveChanges();
        //}

        public IEnumerable<PayableOrderViewModel> CreatePaymentFromOrder(long orderId)
        {
            IQueryable<PayableOrderViewModel> queryBillableOrder =
                from o in DataContext.Orders
                join od in DataContext.OrderDetails on o.Id equals od.OrderId
                join p in DataContext.Products on od.ProductId equals p.Id
                join cat in DataContext.ProductCategories on p.ProductCategoryId equals cat.Id
                where o.Id == orderId
                select new PayableOrderViewModel
                {
                    ProductCode = p.Code,
                    ProductName = p.Name,
                    ProductCategory = cat.Name,
                    Quantity = od.Qty,
                    UnitPrice = od.UnitPrice,
                    DiscPercent = od.DiscPercent,
                    DiscValue = od.DiscValue > 0 && od.DiscPercent == 0 ? od.DiscValue : od.DiscPercent / 100 * od.Qty * od.UnitPrice
                };
            return queryBillableOrder.ToList();
        }

        public bool IsPaid(long orderId)
        {
            IQueryable<Payment> queryPayment = from pay in DataContext.Payments
                                               where !pay.VoidWhen.HasValue
                                                   && pay.OrderId == orderId
                                               select pay;
            return queryPayment.Any();
        }

        public void DeletePayment(long paymentId)
        {
            Payment payment = GetPayment(paymentId);
            DataContext.Payments.Remove(payment);
            DataContext.SaveChanges();
        }

        public void DeletePayment(long[] arrayPaymentId)
        {
            IEnumerable<Payment> payments = DataContext.Payments.Where(it => arrayPaymentId.Contains(it.Id)).ToList();
            DataContext.Payments.RemoveRange(payments);
            DataContext.SaveChanges();
        }

        public Payment GetPayment(long paymentId)
        {
            return DataContext.Payments.Single(entity => entity.Id == paymentId);
        }

        public IEnumerable<Payment> GetPayments(bool onlyActive = true)
        {
            IQueryable<Payment> query = DataContext.Payments;

            return query.ToList();
        }

        public IQueryable<ListPaymentHistoryViewModel> ListPaymentHistory()
        {
            IQueryable<ListPaymentHistoryViewModel> query =
                from pay in DataContext.Payments
                join order in DataContext.Orders on pay.OrderId equals order.Id
                where !order.VoidWhen.HasValue && !pay.VoidWhen.HasValue
                select new ListPaymentHistoryViewModel
                {
                    Id = pay.Id,
                    PaymentNo = pay.PaymentNo,
                    PaymentDate = pay.Date,
                    BillableAmount = pay.BilledAmount,
                    PaidAmount = pay.PaidAmount,
                    OrderType = order.OrderType,
                    OrderDate = order.Date,
                    OrderNo = order.OrderNo,
                    OrderId = order.Id
                };
            return query;
        }

        public IEnumerable<PaymentDetail> GetPaymentDetail(long id)
        {
            return DataContext.PaymentDetails.Where(o => o.PaymentId == id);
        }

        public bool IsAllowVoidPayment(string userName, string password)
        {
            UserLogin userLogin = DataContext.UserLogins.SingleOrDefault(user => user.UserName == userName && user.Password == password && user.AllowVoidPayment);
            if (userLogin != null)
            {
                return userLogin.AllowVoidPayment;
            }

            return false;
        }

        public void VoidPayment(long paymentId, string userName)
        {
            Payment payment = GetPayment(paymentId);
            if (payment != null)
            {
                payment.VoidWhen = DateTime.Now;
                payment.VoidAuth = userName;

                inventoryOutProvider.UnPostFromSales(payment);

                DataContext.SaveChanges();
            }
        }

        public string GetPaymentTypes(int paymentId)
        {
            string paymentTypes = String.Empty;
            if (paymentId > 0)
            {
                Payment payment = GetPayment(paymentId);
                if (payment != null)
                {
                    var list = new List<string>();
                    foreach (var paymentDetail in payment.PaymentDetails)
                    {
                        if(paymentDetail.Amount > 0)
                            list.Add(paymentDetail.PaymentType.Name);
                    }
                    paymentTypes = String.Join(", ", list.ToArray());
                }
            }

            return paymentTypes;
        }
    }
}
