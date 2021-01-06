using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MujiStore.Models;
using MujiStore.BLL;
using PagedList;
using System.IO;

namespace MujiStore.Controllers
{
    [SessionExpire]
    [Authorize]
    [MUJICustomAuthorize(Roles = "16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31")]
    public class FormatsController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: Formats
        public ActionResult Index(int? page)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;

            int pageSize;
            if (System.Configuration.ConfigurationManager.AppSettings["PageSize"] == null)
            {
                pageSize = 10;
            }
            else
            {
                pageSize = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
            }
            ViewBag.PageSizeList = CommonLogic.GetPageSize();

            int pageNumber = (page ?? 1);
            try
            {
                
                    return View(db.tblFormats.ToList().OrderByDescending(x => x.FormatID).ToPagedList(pageNumber, pageSize));
           
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
           
        }

        // GET: Formats/Details/5
        [NonAction]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFormat tblFormat = db.tblFormats.Find(id);
            if (tblFormat == null)
            {
                return HttpNotFound();
            }
            return View(tblFormat);
        }

        // GET: Formats/Create
        public ActionResult Create()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: Formats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FormatID,Name,RequiredBandWidth,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblFormat tblFormat)
        {
 
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;

         

            try
            {
                var sname = db.tblFolders.Where(x => x.Name.ToLower().Trim().ToString() == tblFormat.Name.ToLower().ToString()).FirstOrDefault();
                if (sname != null)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonNameAlreadyExists;
                    return View(tblFormat);
                }

                if (ModelState.IsValid)
                {

                    LogInfo.Comments = MujiStore.Resources.Resource.CommonNameFolderCreated + tblFormat.Name.ToString();

                    tblFormat.DELFG = false;
                    tblFormat.CRTDT = DateTime.Now;
                    tblFormat.CRTCD = Session["UserName"].ToString();
                    tblFormat.IPAddress = Session["IPAddress"].ToString();
                    db.tblFormats.Add(tblFormat);
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CommonCreatedSuccessfully;
                    return RedirectToAction("Index");
                }

                return View(tblFormat);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonUnableCreateFormat;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }

        // GET: Formats/Edit/5
        public ActionResult Edit(int? id)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblFormat folder = db.tblFormats.Find(id);
                if (folder == null)
                {
                    return HttpNotFound();
                }

                return View(folder);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: Formats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FormatID,Name,RequiredBandWidth,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblFormat tblFormat)
        {
 
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
               

                var fname = db.tblFormats.Where(x => x.Name.ToLower().Trim().ToString() == tblFormat.Name.ToLower().ToString() && x.FormatID != tblFormat.FormatID).FirstOrDefault();
                if (fname != null)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonNameUpdateFormatNameAlreadyExists;
                    return View(tblFormat);
                }

               
                if (ModelState.IsValid)
                {

                    LogInfo.Comments = "Format Updated - " + tblFormat.Name.ToString();
                    tblFormat.UPDDT = DateTime.Now;
                    tblFormat.UPDCD = Session["UserName"].ToString();
                    tblFormat.IPAddress = Session["IPAddress"].ToString();
                    db.Entry(tblFormat).State = EntityState.Modified;
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CommonNameUpdateFormatUpdateSuc;
                    return RedirectToAction("Index");
                }

                TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonNameUpdateFormatUpdateForm;
                return View(tblFormat);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonNameUpdateFormatUpdateForm;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: Formats/Delete/5
        [NonAction]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFormat tblFormat = db.tblFormats.Find(id);
            if (tblFormat == null)
            {
                return HttpNotFound();
            }
            return View(tblFormat);
        }

        // POST: Formats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NonAction]
        public ActionResult DeleteConfirmed(int id)
        {
            tblFormat tblFormat = db.tblFormats.Find(id);
            db.tblFormats.Remove(tblFormat);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
