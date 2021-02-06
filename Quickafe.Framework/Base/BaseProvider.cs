using Quickafe.DataAccess;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace Quickafe.Framework.Base
{
    public abstract class BaseProvider
    {
        private static System.Lazy<Dictionary<bool, Action<dynamic, string>>> auditFieldsAssigner
            = new Lazy<Dictionary<bool, Action<dynamic, string>>>(() =>
            new Dictionary<bool, Action<dynamic, string>>()
            {
                [true] = SetAuditFieldsForInsert,
                [false] = SetAuditFieldsForUpdate
            });

        private QuickafeDbContext db;

        protected QuickafeDbContext DataContext => db;

        public BaseProvider(IQuickafeDbContext db)
        {
            this.db = (QuickafeDbContext)db;
        }

        public virtual ClaimsPrincipal Principal { get; set; }

        protected virtual string CurrentUserName => 
            Principal == null ? "guest" : Principal.Identity.Name;

        protected virtual void SetAuditFields(dynamic entity, string userName = null) => 
            auditFieldsAssigner.Value[entity.Id == 0](entity, userName ?? CurrentUserName);

        private static void SetAuditFieldsForInsert(dynamic entity, string userName)
        {
            entity.CreatedWhen = DateTime.Now;
            entity.ChangedWhen = entity.CreatedWhen;
            entity.CreatedBy = userName;
            entity.ChangedBy = userName;
        }

        private static void SetAuditFieldsForUpdate(dynamic entity, string userName)
        {
            entity.ChangedWhen = entity.CreatedWhen;
            entity.ChangedBy = userName;
        }
    }
    
}
