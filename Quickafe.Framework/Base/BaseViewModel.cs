using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Framework.Base
{
    [Serializable]
    public abstract class BaseViewModel : BaseConcurrencyViewModel
    {
        public DateTime CreatedWhen { get; set; }
        public DateTime ChangedWhen { get; set; }
        public string CreatedBy { get; set; }
        public string ChangedBy { get; set; }
    }
}
