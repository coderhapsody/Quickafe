using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quickafe.Providers.Inventory.ViewModels.InventoryIn
{
    public class CreateEditViewModel : BaseViewModel
    {
        public long Id { get; set; }
        public long LocationId { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Maximum character is 20 chars")]
        [Display(Name ="Inventory No")]
        public string InventoryNo { get; set; }
        [Required]
        [Display(Name ="Inventory Date")]
        public DateTime InventoryDate { get; set; }

        [Required]
        public string InventoryType { get; set; }
        [Required]
        public string MutationType { get; set; }
        public string Direction { get; set; }
        public string Notes { get; set; }
        public IEnumerable<InventoryDetailEntryViewModel> InventoryDetails { get; set; }
    }
}
