using System;

namespace Quickafe.Providers.ViewModels.User
{
    public class ListUserLoginViewModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }        
        public DateTime? LastLogin { get; set; }
        public DateTime? LastChangePassword { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; }
        public bool IsSystemUser { get; set; }

        public bool AllowVoidPayment { get; set; }
        public bool AllowVoidOrder { get; set; }
        public bool AllowPrintReceipt { get; set; }
    }
}
