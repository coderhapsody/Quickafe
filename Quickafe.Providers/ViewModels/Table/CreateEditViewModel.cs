using Quickafe.Framework.Base;
using System.ComponentModel.DataAnnotations;

namespace Quickafe.Providers.ViewModels.Table
{
    public class CreateEditViewModel : BaseViewModel
    {
        public long Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
