using Quickafe.Framework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quickafe.DataAccess;
using LinqKit;
using System.Linq.Expressions;

namespace Quickafe.Providers
{
    public class AutoNumberProvider : BaseProvider
    {
        public AutoNumberProvider(IQuickafeDbContext db) : base(db)
        {
        }

        private AutoNumber GetAutoNumberInstance(string moduleCode, int? year = null, int? month = null)
        {
            Expression<Func<AutoNumber, bool>> predicate = PredicateBuilder.True<AutoNumber>();
            predicate = predicate.And(p => p.ModuleCode == moduleCode);
            if (year.HasValue)
                predicate = predicate.And(p => p.Year == year.Value);
            if (month.HasValue)
                predicate = predicate.And(p => p.Month == month.Value);

            AutoNumber autoNumberInstance = DataContext.AutoNumbers.AsExpandable().FirstOrDefault(predicate);
            if (autoNumberInstance == null)
            {
                using (var context = new QuickafeDbContext())
                {
                    autoNumberInstance = new AutoNumber
                    {
                        ModuleCode = moduleCode,
                        Year = year.GetValueOrDefault(0),
                        Month = month.GetValueOrDefault(0),
                        LastNumber = 0
                    };
                    context.AutoNumbers.Add(autoNumberInstance);
                    context.SaveChanges();
                }
            }
            return autoNumberInstance;
        }

        public long GetLastNumber(string moduleCode, int? year = null, int? month = null)
        {
            AutoNumber autoNumber = GetAutoNumberInstance(moduleCode, year, month);
            return autoNumber.LastNumber;                
        }

        public void IncrementLastNumber(string moduleCode, int? year = null, int? month = null)
        {
            AutoNumber autoNumber = GetAutoNumberInstance(moduleCode, year, month);
            autoNumber.LastNumber++;
        }
    }
}
