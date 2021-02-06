using Quickafe.Framework.Base;
using Quickafe.Providers.ViewModels.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quickafe.DataAccess;
using System.Globalization;

namespace Quickafe.Providers
{
    public class ChartProvider : BaseProvider
    {
        public ChartProvider(IQuickafeDbContext db) : base(db)
        {
        }

        public IEnumerable<MtdOrderVolumeViewModel> GetMtmOrderVolume(int year)
        {
            var query = (from o in DataContext.Orders
                         join od in DataContext.OrderDetails on o.Id equals od.OrderId
                         where o.Date.Year == year && !o.VoidWhen.HasValue
                         group od by o.Date.Month into g
                         select new
                         {
                             Month = g.Key,
                             Volume = g.Sum(d => d.Qty * d.UnitPrice - (d.DiscPercent > 0 ? d.DiscPercent / 100 * d.Qty * d.UnitPrice : d.DiscValue))
                         }).OrderBy(o => o.Month).ToList();
            foreach (var row in query)
            {
                yield return new MtdOrderVolumeViewModel
                {
                    Month = row.Month,
                    MonthName = new DateTime(1900, row.Month, 1).ToString("MMMM", CultureInfo.GetCultureInfo("id-ID")),
                    Volume = row.Volume
                };
            }
        }
    }
}
