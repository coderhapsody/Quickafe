using System.Web;
using System.Web.Mvc;

namespace Quickafe.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
#if AUTHORIZATION
            filters.Add(new AuthorizeAttribute());
#endif
        }
    }
}
