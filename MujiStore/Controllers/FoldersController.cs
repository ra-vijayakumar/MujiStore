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
    public class FoldersController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();
     
    
        // GET: Folders
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
  
                var Folder = (from folder in db.tblFolders
                              join parent in db.tblFolders on folder.ParentID equals parent.FolderID
                              where folder.FolderID != 1
                              select new
                              {
                                  FolderID = folder.FolderID,
                                  Name = folder.Name,
                                  ParentID = parent.FolderID,
                                  ParentFolderName = parent.Name,
                                  DELFG = folder.DELFG
                              }).ToList().OrderByDescending(x => x.FolderID);//.ToPagedList(pageNumber, pageSize);

                List<tblFolder> folderlist = new List<tblFolder>();
                foreach (var items in Folder)
                {
                    tblFolder folder = new tblFolder();
                    folder.FolderID = items.FolderID;
                    folder.Name = items.Name;
                    folder.ParentID = items.ParentID;
                    folder.ParentFolderName = items.ParentFolderName;
                    folder.DELFG = items.DELFG;
                    folderlist.Add(folder);
                }
                
                

                return View(folderlist.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: Folders/Details/5
        [NonAction]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFolder folder = db.tblFolders.Find(id);
            if (folder == null)
            {
                return HttpNotFound();
            }
            return View(folder);
        }

        // GET: Folders/Create
        public ActionResult Create()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                List<SelectListItem> lstFolderName = new List<SelectListItem>();
                lstFolderName = BLL.CommonLogic.FillFolderList();
                //lstFolderName.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
                ViewBag.ParentID = new SelectList(lstFolderName, "Value", "Text");
                // ViewBag.ParentID = new SelectList(db.tblFolders.Where(x => x.DELFG == false), "FolderID", "Name");
                //ViewBag.ParentID = new SelectList(BLL.CommonLogic.FillFolderList());
                //ViewBag.ParentID = BLL.CommonLogic.FillFolderList();
                return View();
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: Folders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FolderID,ParentID,Name")] tblFolder folder)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            
            //ViewBag.ParentID = new SelectList(db.tblFolders.Where(x => x.DELFG == false), "FolderID", "Name", folder.FolderID);
            List<SelectListItem> lstFolderName = new List<SelectListItem>();
            lstFolderName = BLL.CommonLogic.FillFolderList();
            ViewBag.ParentID = new SelectList(lstFolderName, "Value", "Text", folder.FolderID);
            try
            {
                var sname = db.tblFolders.Where(x => x.Name.ToLower().Trim().ToString() == folder.Name.ToLower().ToString()).FirstOrDefault();
                if (sname != null)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntFoldersCreateErrMsg;
                    return View(folder);
                }

                if (ModelState.IsValid)
                {
                   
                    LogInfo.Comments = MujiStore.Resources.Resource.CommonNameFolderCreated + folder.Name.ToString();

                    folder.DELFG = false;
                    folder.CRTDT = DateTime.Now;
                    folder.CRTCD = Session["UserName"].ToString();
                    folder.IPAddress = Session["IPAddress"].ToString();
                    db.tblFolders.Add(folder);
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntFoldersCreateSuccMsg;
                    return RedirectToAction("Index");
                }
                
                return View(folder);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonNameUnableCreateFolder;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: Folders/Edit/5
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
                tblFolder folder = db.tblFolders.Find(id);
                if (folder == null)
                {
                    return HttpNotFound();
                }

                List<SelectListItem> lstFolderName = new List<SelectListItem>();
                lstFolderName = BLL.CommonLogic.FillFolderList();
                ViewBag.ParentID = new SelectList(lstFolderName, "Value", "Text", folder.ParentID);

                //ViewBag.ParentID = new SelectList(db.tblFolders.Where(x => x.DELFG == false), "FolderID", "Name", folder.ParentID);
                return View(folder);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: Folders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FolderID,ParentID,Name,DELFG,CRTDT,CRTCD")] tblFolder folder)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                List<SelectListItem> lstFolderName = new List<SelectListItem>();
                lstFolderName = BLL.CommonLogic.FillFolderList();
                ViewBag.ParentID = new SelectList(lstFolderName, "Value", "Text", folder.ParentID);

                //lViewBag.ParentID = new SelectList(db.tblFolders.Where(x => x.DELFG == false), "FolderID", "Name", folder.ParentID);

                var fname = db.tblFolders.Where(x => x.Name.ToLower().Trim().ToString() == folder.Name.ToLower().ToString() && x.FolderID != folder.FolderID).FirstOrDefault();
                if (fname != null)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntFoldersEditErrMsg1;
                    return View(folder);
                }

                var pcCheck = db.tblFolders.Where(x => x.ParentID == folder.FolderID && x.FolderID == folder.ParentID).FirstOrDefault();
                if (pcCheck != null)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntFoldersEditErrMsg2;
                    return View(folder);
                }
                if (ModelState.IsValid)
                {

                    LogInfo.Comments = @MujiStore.Resources.Resource.CommonFolderUpdated + folder.Name.ToString();
                    folder.UPDDT = DateTime.Now;
                    folder.UPDCD = Session["UserName"].ToString();
                    folder.IPAddress = Session["IPAddress"].ToString();
                    db.Entry(folder).State = EntityState.Modified;
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntFoldersEditSuccMsg1;
                    return RedirectToAction("Index");
                }
                
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntFoldersEditErrMsg3;
                return View(folder);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonFolderUnableUpdateFolder;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }

        // GET: Folders/Delete/5
        [NonAction]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFolder folder = db.tblFolders.Find(id);
            if (folder == null)
            {
                return HttpNotFound();
            }
            return View(folder);
        }

        // POST: Folders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NonAction]
        public ActionResult DeleteConfirmed(int id)
        {
            tblFolder folder = db.tblFolders.Find(id);
            db.tblFolders.Remove(folder);
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
