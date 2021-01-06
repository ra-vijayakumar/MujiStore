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

namespace MujiStore.Controllers
{
    [SessionExpire]
    [Authorize]
    [MUJICustomAuthorize(Roles = "16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31")]
    public class StoreGroupFoldersController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: StoreGroupFolders
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

            int pageNumber = (page ?? 1);

            try
            {
                return View(db.tblStoreGroupFolders.ToList().OrderByDescending(x => x.StoreGroupFolderID).ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        [NonAction]
        // GET: StoreGroupFolders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStoreGroupFolder tblStoreGroupFolder = db.tblStoreGroupFolders.Find(id);
            if (tblStoreGroupFolder == null)
            {
                return HttpNotFound();
            }
            return View(tblStoreGroupFolder);
        }

        // GET: StoreGroupFolders/Create
        public ActionResult Create()
        {
            

            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                ViewBag.FolderID = new SelectList(db.tblFolders.Where(x =>x.DELFG == false), "FolderID", "Name");
                ViewBag.StoreGroupID = new SelectList(db.tblStoreGroups.Where(x => x.DELFG == false), "StoreGroupID", "Name");
                return View();
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: StoreGroupFolders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StoreGroupFolderID,StoreGroupID,FolderID,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblStoreGroupFolder tblStoreGroupFolder)
        {
          
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;

            try
            {
                ViewBag.FolderID = new SelectList(db.tblFolders.Where(x => x.DELFG == false), "FolderID", "Name", tblStoreGroupFolder.FolderID);
                ViewBag.StoreGroupID = new SelectList(db.tblStoreGroups.Where(x => x.DELFG == false), "StoreGroupID", "Name", tblStoreGroupFolder.StoreGroupID);

                if (ModelState.IsValid)
                {
                    LogInfo.Comments = @MujiStore.Resources.Resource.StoreGroupCreateFolderCreate + tblStoreGroupFolder.StoreGroupID + ","+ tblStoreGroupFolder.FolderID;

                    tblStoreGroupFolder.CRTDT = DateTime.Now;
                    tblStoreGroupFolder.CRTCD = Session["UserName"].ToString();
                    tblStoreGroupFolder.IPAddress = Session["IPAddress"].ToString();
                    tblStoreGroupFolder.DELFG = false;
                    //tblVideoDemoStoreMst.UdDate = DateTime.Now;
                    db.tblStoreGroupFolders.Add(tblStoreGroupFolder);
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntStoreGroupFoldersCreateSuccMsg;
                    return RedirectToAction("Index");
                }

                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupFoldersCreateErrMsg1;
 
                return View(tblStoreGroupFolder);
            }
            catch (Exception ex)
            {

                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupFoldersCreateErrMsg2;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupFoldersCreateErrMsg3;
                }

                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
     
        }

        // GET: StoreGroupFolders/Edit/5
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
                tblStoreGroupFolder tblStoreGroupFolder = db.tblStoreGroupFolders.Find(id);

                if (tblStoreGroupFolder == null)
                {
                    return HttpNotFound();
                }
                ViewBag.FolderID = new SelectList(db.tblFolders.Where(x => x.DELFG == false), "FolderID", "Name", tblStoreGroupFolder.FolderID);
                ViewBag.StoreGroupID = new SelectList(db.tblStoreGroups.Where(x => x.DELFG == false), "StoreGroupID", "Name", tblStoreGroupFolder.StoreGroupID);
                return View(tblStoreGroupFolder);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }

        // POST: StoreGroupFolders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StoreGroupFolderID,StoreGroupID,FolderID,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblStoreGroupFolder tblStoreGroupFolder)
        {
         
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                ViewBag.FolderID = new SelectList(db.tblFolders.Where(x => x.DELFG == false), "FolderID", "Name", tblStoreGroupFolder.FolderID);
                ViewBag.StoreGroupID = new SelectList(db.tblStoreGroups.Where(x => x.DELFG == false), "StoreGroupID", "Name", tblStoreGroupFolder.StoreGroupID);

                if (ModelState.IsValid)
                {

                    LogInfo.Comments = MujiStore.Resources.Resource.CntStoreGroupUpdateCommon + tblStoreGroupFolder.StoreGroupFolderID.ToString();

                    tblStoreGroupFolder.UPDDT = DateTime.Now;
                    tblStoreGroupFolder.UPDCD = Session["UserName"].ToString();
                    tblStoreGroupFolder.IPAddress = Session["IPAddress"].ToString();
                    db.Entry(tblStoreGroupFolder).State = EntityState.Modified;
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntStoreGroupFoldersEditSuccMsg;
                    return RedirectToAction("Index");
                }
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupFoldersEditErrMsg1;
       
                return View(tblStoreGroupFolder);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupFoldersEditErrMsg2;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupFoldersEditErrMsg3;
                }
                //TempData["ErrMsg"] = "Unable Update Store Group";
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        [NonAction]
        // GET: StoreGroupFolders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStoreGroupFolder tblStoreGroupFolder = db.tblStoreGroupFolders.Find(id);
            if (tblStoreGroupFolder == null)
            {
                return HttpNotFound();
            }
            return View(tblStoreGroupFolder);
        }
        [NonAction]
        // POST: StoreGroupFolders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblStoreGroupFolder tblStoreGroupFolder = db.tblStoreGroupFolders.Find(id);
            db.tblStoreGroupFolders.Remove(tblStoreGroupFolder);
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
