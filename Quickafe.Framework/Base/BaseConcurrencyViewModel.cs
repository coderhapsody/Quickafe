using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Framework.Base
{
    public class BaseConcurrencyViewModel
    {
        public byte[] RowVersion { get; set; } = new byte[] { };
    }
}
