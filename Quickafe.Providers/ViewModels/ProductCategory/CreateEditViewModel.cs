using Quickafe.Framework.Base;
using System.ComponentModel.DataAnnotations;

namespace Quickafe.Providers.ViewModels.ProductCategory
{
    public class CreateEditViewModel : BaseViewModel
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Show in Order view")]
        public bool CanOrder { get; set; }
    }
}
