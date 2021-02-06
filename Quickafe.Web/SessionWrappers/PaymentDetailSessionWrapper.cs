using Quickafe.Providers.Sales.ViewModels.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Quickafe.Web.SessionWrappers
{
    public class PaymentDetailSessionWrapper
    {
        internal static HttpSessionState CurrentSession => HttpContext.Current.Session;

        internal static List<PaymentDetailViewModel> Detail
        {
            get
            {
                if (CurrentSession["PaymentDetail"] == null)
                    Initialize();
                return CurrentSession["PaymentDetail"] as List<PaymentDetailViewModel>;
            }
        }

        internal static void Initialize() => CurrentSession["PaymentDetail"] = new List<PaymentDetailViewModel>();

        internal static void Initialize(IEnumerable<PaymentDetailViewModel> detailList)
        {
            Initialize();
            Detail.AddRange(detailList);
        }

        internal static void AddDetail(PaymentDetailViewModel detailLine)
        {
            detailLine.Uid = Guid.NewGuid();
            Detail.Add(detailLine);
        }

        internal static PaymentDetailViewModel GetDetail(Guid uid) => Detail.Single(d => d.Uid == uid);

        internal static void DeleteDetatil(Guid uid) => Detail.RemoveAll(p => p.Uid == uid);

        internal static bool IsEmpty => !Detail.Any();

        internal static void UpdateDetail(PaymentDetailViewModel model)
        {
            DeleteDetatil(model.Uid);
            AddDetail(model);
        }

        internal static decimal TotalPayment => Detail.Sum(p => p.Amount);

        internal static void Clear() => Detail.Clear();

        internal static decimal TotalCash => Detail.Where(p => p.PaymentTypeId == 1).Sum(p => p.Amount);

        internal static decimal TotalNonCash => Detail.Where(p => p.PaymentTypeId > 1).Sum(p => p.Amount);

    }
}