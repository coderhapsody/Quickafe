using Quickafe.Framework.Base;
using System.ComponentModel.DataAnnotations;

namespace Quickafe.Providers.ViewModels.Product
{
    public class CreateEditViewModel : BaseViewModel
    {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Product Code")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Display(Name = "Product Category")]
        public int ProductCategoryId { get; set; }

        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Unit Price 2")]
        public decimal UnitPrice2 { get; set; }

        [Display(Name = "Unit Price 3")]
        public decimal UnitPrice3 { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Yield Product")]
        public long? YieldProductId { get; set; }
    }
}
