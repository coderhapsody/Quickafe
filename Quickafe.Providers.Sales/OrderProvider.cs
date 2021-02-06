using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Providers.Sales.ViewModels.Order;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Quickafe.Providers.Sales
{
    public class OrderProvider : BaseProvider
    {
        private readonly IMapper mapper;
        private readonly AutoNumberProvider autoNumberProvider;
        private readonly ConfigurationProvider configurationProvider;

        public OrderProvider(IQuickafeDbContext context, IMapper mapper, AutoNumberProvider autoNumberProvider, ConfigurationProvider configurationProvider) : base(context)
        {
            this.mapper = mapper;
            this.autoNumberProvider = autoNumberProvider;
            this.configurationProvider = configurationProvider;
        }

        public void AddOrder(Order order)
        {
            object o = new object();

            DataContext.Orders.Add(order);
            lock (o)
            {
                long lastNumber = autoNumberProvider.GetLastNumber("ORDER", order.Date.Year);
                string prefix = configurationProvider.GetConfiguration<string>(ConfigurationKeys.OrderPrefix);
                int length = configurationProvider.GetConfiguration<int>(ConfigurationKeys.OrderNumberingLength);
                lastNumber++;
                string orderNo = String.Format("{0}{1}", prefix, lastNumber.ToString(String.Concat(Enumerable.Repeat("0", length))));
                order.OrderNo = orderNo;
                autoNumberProvider.IncrementLastNumber("ORDER", order.Date.Year);
                SetAuditFields(order);
            }
            DataContext.SaveChanges();
        }

        public Dictionary<int, string> GetUnitPriceModes()
        {
            var unitPriceModes = new Dictionary<int, string>();

            if (!String.IsNullOrEmpty(configurationProvider[ConfigurationKeys.UnitPriceMode1]))
                unitPriceModes.Add(1, configurationProvider[ConfigurationKeys.UnitPriceMode1]);

            if (!String.IsNullOrEmpty(configurationProvider[ConfigurationKeys.UnitPriceMode2]))
                unitPriceModes.Add(2, configurationProvider[ConfigurationKeys.UnitPriceMode2]);

            if (!String.IsNullOrEmpty(configurationProvider[ConfigurationKeys.UnitPriceMode3]))
                unitPriceModes.Add(3, configurationProvider[ConfigurationKeys.UnitPriceMode3]);

            return unitPriceModes;
        }

        public string GetUnitPriceMode(int unitPriceMode)
        {
            var pricingModes = GetUnitPriceModes();
            return pricingModes.Count > 0 && unitPriceMode <= pricingModes.Count ?
                pricingModes[unitPriceMode] : String.Empty;
        }

        public void UpdateOrder(Order order)
        {
            var oldOrderDetail = DataContext.OrderDetails.Where(o => o.OrderId == order.Id).ToList();
            DataContext.OrderDetails.RemoveRange(oldOrderDetail);
            foreach (OrderDetail orderDetail in order.OrderDetails)
                DataContext.OrderDetails.Add(orderDetail);
            SetAuditFields(order);
            DataContext.SaveChanges();
        }

        public void DeleteOrder(long orderId)
        {
            Order order = GetOrder(orderId);
            DataContext.Orders.Remove(order);
            DataContext.SaveChanges();
        }

        public void DeleteOrder(long[] arrayOrderId)
        {
            IEnumerable<Order> orders = DataContext.Orders.Where(it => arrayOrderId.Contains(it.Id)).ToList();
            DataContext.Orders.RemoveRange(orders);
            DataContext.SaveChanges();
        }

        public Order GetOrder(long orderId)
        {
            return DataContext.Orders.Single(entity => entity.Id == orderId);
        }

        public IEnumerable<OrderDetail> GetOrderDetail(long orderId)
        {
            return DataContext.OrderDetails.Where(o => o.OrderId == orderId);
        }

        public IQueryable<ListOrderHistoryViewModel> ListOrderHistory()
        {
            IQueryable<ListOrderHistoryViewModel> query =
                from order in DataContext.Orders
                join table in DataContext.Tables on order.TableId equals table.Id
                join pay in DataContext.Payments on order.Id equals pay.OrderId into gpay
                from p in gpay.DefaultIfEmpty()
                //where !order.VoidWhen.HasValue && !p.VoidWhen.HasValue
                select new ListOrderHistoryViewModel
                {
                    Id = order.Id,
                    OrderNo = order.OrderNo,
                    OrderType = order.OrderType,
                    Date = order.Date,
                    TableCode = table.Code,
                    PaymentNo = p.PaymentNo,
                    PaymentDate = p.Date,
                    TotalPayment = p.PaidAmount,
                    TotalOrder = order.OrderDetails.Sum(o => o.Qty * o.UnitPrice - (o.DiscPercent > 0 ? o.DiscPercent / 100 * o.Qty * o.UnitPrice : o.DiscValue)),
                    VoidWhen = order.VoidWhen, 
                    IsOrderVoid = order.VoidWhen.HasValue            
                };

            return query;
        }

        public IQueryable<ListOrderViewModel> ListOutstandingOrders()
        {
            var activePayments = DataContext.Payments.Where(p => !p.VoidWhen.HasValue).Select(p => p.Order);
           
            IQueryable<ListOrderViewModel> query = 
                from order in DataContext.Orders.Include(o => o.Payments)
                join table in DataContext.Tables on order.TableId equals table.Id
                where (!order.Payments.Any() || !activePayments.Contains(order)) && !order.VoidWhen.HasValue                
                select new ListOrderViewModel
                {
                    Id = order.Id,
                    OrderNo = order.OrderNo,
                    OrderType = order.OrderType,
                    OrderDetailCount = order.OrderDetails.Any() ? order.OrderDetails.Sum(od => od.Qty) : 0,
                    Date = order.Date,
                    TableCode = table.Code,
                    Guests = order.Guests,
                    ChangedWhen = order.ChangedWhen
                };

            return query;
        }
        
        public bool IsAllowPrintReceipt(string userName)
        {
            UserLogin userLogin = DataContext.UserLogins.SingleOrDefault(user => user.UserName == userName);
            if(userLogin != null)
            {
                return userLogin.AllowPrintReceipt;
            }

            return false;
        }

        public bool IsAllowVoidOrder(string userName, string password)
        {
            UserLogin userLogin = DataContext.UserLogins.SingleOrDefault(user => user.UserName == userName && user.Password == password && user.AllowVoidOrder);
            if (userLogin != null)
            {
                return userLogin.AllowVoidOrder;
            }

            return false;
        }
       
        public void VoidOrder(long orderId, string userName)
        {
            Order order = GetOrder(orderId);
            if(order != null)
            {
                order.VoidWhen = DateTime.Now;
                order.VoidAuth = userName;
                DataContext.SaveChanges();
            }
        }
    }
}
