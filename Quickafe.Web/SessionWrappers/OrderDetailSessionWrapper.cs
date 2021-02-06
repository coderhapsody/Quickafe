using Quickafe.Providers.Sales.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Quickafe.Web.SessionWrappers
{
    public static class OrderDetailSessionWrapper
    {
        internal static HttpSessionState CurrentSession => HttpContext.Current.Session;

        internal static List<OrderDetailEntryViewModel> Detail
        {
            get
            {
                if (CurrentSession["OrderDetail"] == null)
                    Initialize();
                return CurrentSession["OrderDetail"] as List<OrderDetailEntryViewModel>;
            }
        }

        internal static void Initialize() => CurrentSession["OrderDetail"] = new List<OrderDetailEntryViewModel>();

        internal static void Initialize(IEnumerable<OrderDetailEntryViewModel> detailList)
        {
            Initialize();
            Detail.AddRange(detailList);
        }

        internal static void AddDetail(OrderDetailEntryViewModel detailLine)
        {
            detailLine.Uid = Guid.NewGuid();
            detailLine.DetailDiscValue = detailLine.DetailDiscPercent > 0 ? (detailLine.DetailDiscPercent / 100) * detailLine.Qty * detailLine.UnitPrice : detailLine.DetailDiscValue;
            Detail.Add(detailLine);
        }

        internal static OrderDetailEntryViewModel GetDetail(Guid uid) => Detail.Single(d => d.Uid == uid);

        internal static void DeleteDetatil(Guid uid) => Detail.RemoveAll(p => p.Uid == uid);

        internal static bool IsEmpty => !Detail.Any();

        public static decimal TotalOrder => Detail.Sum(o => o.Qty * o.UnitPrice - (o.DetailDiscPercent > 0 ? o.DetailDiscPercent / 100 * o.Qty * o.UnitPrice : o.DetailDiscValue));

        internal static void UpdateDetail(OrderDetailEntryViewModel model)
        {
            DeleteDetatil(model.Uid);
            AddDetail(model);
        }

        internal static void Clear() => Detail.Clear();
    }
}