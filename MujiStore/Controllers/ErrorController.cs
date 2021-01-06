using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MujiStore.Models;
using MujiStore.BLL;

namespace MujiStore.Controllers
{
    [HandleError]
    public class ErrorController : Controller
    {
        
        public ActionResult Error()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                ErrorInfo errorInfo = new ErrorInfo();
                errorInfo.Message = MujiStore.Resources.Resource.CntErrorMessage;
                errorInfo.Description = MujiStore.Resources.Resource.CntErrorDescription;
                return PartialView(errorInfo);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        public ActionResult BadRequest()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = MujiStore.Resources.Resource.CntErrorBadRequestMessage;
            errorInfo.Description = MujiStore.Resources.Resource.CntErrorBadRequestDescription;
            return PartialView("Error", errorInfo);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        public ActionResult NotFound()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = MujiStore.Resources.Resource.CntErrorNotFoundMessage;
            errorInfo.Description = MujiStore.Resources.Resource.CntErrorNotFoundDescription;
            return PartialView("Error", errorInfo);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        public ActionResult Forbidden()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = MujiStore.Resources.Resource.CntErrorForbiddenMessage;
            errorInfo.Description = MujiStore.Resources.Resource.CntErrorForbiddenDescription;
            return PartialView("Error", errorInfo);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }
        public ActionResult URLTooLong()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
             ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = MujiStore.Resources.Resource.CntErrorURLTooLongMessage;
            errorInfo.Description = MujiStore.Resources.Resource.CntErrorURLTooLongDescription;
            return PartialView("Error", errorInfo);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        public ActionResult ServiceUnavailable()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.Message = MujiStore.Resources.Resource.CntErrorServiceUnavailableMessage;
            errorInfo.Description = MujiStore.Resources.Resource.CntErrorServiceUnavailableDescription;
            return PartialView("Error", errorInfo);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}