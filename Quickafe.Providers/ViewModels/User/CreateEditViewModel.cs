using System.ComponentModel.DataAnnotations;

namespace Quickafe.Providers.ViewModels.User
{
    public class CreateEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        public bool IsActive { get; set; }
        public bool AllowVoidOrder { get; set; }
        public bool AllowVoidPayment { get; set; }
        public bool AllowPrintReceipt { get; set; }

        public bool IsSystemUser { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
