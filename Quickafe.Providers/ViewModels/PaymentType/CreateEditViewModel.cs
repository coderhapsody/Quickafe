using Quickafe.Framework.Base;
using System.ComponentModel.DataAnnotations;

namespace Quickafe.Providers.ViewModels.PaymentType
{
    public class CreateEditViewModel : BaseViewModel
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
