using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.ViewModels
{
    public class CacheDataModel
    {
        public CacheDataModel(IList<RoleAccessCacheModel> roleAccessCacheData)
        {
            RoleAccess = roleAccessCacheData;
        }

        public IList<RoleAccessCacheModel> RoleAccess { get; private set; }
    }
}
