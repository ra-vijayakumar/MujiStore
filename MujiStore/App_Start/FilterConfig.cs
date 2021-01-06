using System.Web;
using System.Web.Mvc;

namespace MujiStore
{
    public class FilterConfig
    {
      
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
