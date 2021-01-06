using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MujiStore.BLL
{
    public class CustomExceptionHandling : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext exceptionContext)
        {
            if (!exceptionContext.ExceptionHandled)
            {
                string controllerName = (string)exceptionContext.RouteData.Values["controller"];
                string actionName = (string)exceptionContext.RouteData.Values["action"];

                Exception custException = new Exception(MujiStore.Resources.Resource.CustomException1);



                var model = new HandleErrorInfo(custException, controllerName, actionName);

                exceptionContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/Error403.cshtml",
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = exceptionContext.Controller.TempData
                };

                exceptionContext.ExceptionHandled = true;

            }
            
            // test comments on 2021-01-05 _04
        }
    }

    public class Constants
    {
        public readonly string CUSTOM_ERROR1 =  MujiStore.Resources.Resource.CustomException2;
        public readonly string CUSTOM_ERROR2 = MujiStore.Resources.Resource.CustomException3;
    }


}
