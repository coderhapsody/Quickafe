using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.ViewModels.RoleAccess
{
    public class RoleAccessViewModel
    {
        public long MenuId { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsAllowed { get; set; }
    }
}
