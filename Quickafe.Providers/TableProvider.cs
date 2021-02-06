using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Resources;

namespace Quickafe.Providers 
{
	public class TableProvider : BaseProvider
	{
		public TableProvider(IQuickafeDbContext context) : base(context)
        {
        }

        public void AddTable(Table table)
        {
            if (DataContext.Tables.Any(t => t.Code == table.Code))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, table.Code));

            DataContext.Tables.Add(table);
            SetAuditFields(table);
            DataContext.SaveChanges();
        }

        public void UpdateTable(Table table)
        {
            if(DataContext.Tables.Any(t => t.Code == table.Code && t.Id != table.Id))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, table.Code));

            DataContext.Tables.Attach(table);
            DataContext.Entry(table).State = EntityState.Modified;
            SetAuditFields(table);
            DataContext.SaveChanges();
        }

        public void DeleteTable(long tableId)
        {
            Table table = GetTable(tableId);
            DataContext.Tables.Remove(table);
            DataContext.SaveChanges();
        }

        public void DeleteTable(long[] arrayTableId)
        {
            IEnumerable<Table> tables = DataContext.Tables.Where(it => arrayTableId.Contains(it.Id)).ToList();
            DataContext.Tables.RemoveRange(tables);
            DataContext.SaveChanges();
        }

        public Table GetTable(long tableId)
        {
            return DataContext.Tables.Single(entity => entity.Id == tableId);
        }

        public IQueryable<Table> GetTables(bool onlyActive = false)
        {
            DataContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<Table> query = DataContext.Tables;

            if (onlyActive)
                query = query.Where(it => it.IsActive);

            return query;
        }
    }
}
