using Quickafe.Providers.Inventory.ViewModels.InventoryOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Quickafe.Web.SessionWrappers
{
    public static class InventoryOutDetailSessionWrapper
    {

        internal static HttpSessionState CurrentSession => HttpContext.Current.Session;

        internal static List<InventoryDetailEntryViewModel> Detail
        {
            get
            {
                if (CurrentSession["InventoryOutDetail"] == null)
                    Initialize();
                return CurrentSession["InventoryOutDetail"] as List<InventoryDetailEntryViewModel>;
            }
        }

        internal static void Initialize() => CurrentSession["InventoryOutDetail"] = new List<InventoryDetailEntryViewModel>();

        internal static void Initialize(IEnumerable<InventoryDetailEntryViewModel> detailList)
        {
            Initialize();
            Detail.AddRange(detailList);
        }

        internal static void AddDetail(InventoryDetailEntryViewModel detailLine)
        {
            if (Detail.Count == 0)
            {
                detailLine.id = 1;
            }
            else
            {
                var dtl = Detail.LastOrDefault();
                detailLine.id = dtl.id + 1;
            }
            Detail.Add(detailLine);
        }

        internal static InventoryDetailEntryViewModel GetDetail(long id) => Detail.Single(d => d.id == id);

        internal static void DeleteDetail(long id) => Detail.RemoveAll(p => p.id == id);

        internal static bool IsEmpty => !Detail.Any();

        internal static void UpdateDetail(InventoryDetailEntryViewModel model)
        {
            DeleteDetail(model.id);
            AddDetail(model);
        }
    }
}