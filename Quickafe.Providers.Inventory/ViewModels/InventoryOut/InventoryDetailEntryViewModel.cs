using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quickafe.Framework.Base;
using System.ComponentModel.DataAnnotations;

namespace Quickafe.Providers.Inventory.ViewModels.InventoryOut
{
    public class InventoryDetailEntryViewModel : BaseViewModel
    {
        public long id { get; set; }
        [Required]
        public long ProductId { get; set; }
        [Display(Name = "Product")]
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public long UomId { get; set; }
        public string UOMCode { get; set; }
        public string UOMName { get; set; }
        public short Qty { get; set; }
        [Display(Name = "Notes")]
        public string NotesDtl { get; set; }
    }
}
