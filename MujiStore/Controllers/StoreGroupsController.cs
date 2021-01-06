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
    public class StoreGroupsController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: StoreGroups
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
                return View(db.tblStoreGroups.ToList().OrderByDescending(x => x.StoreGroupID).ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
            
        }
        [NonAction]
        // GET: StoreGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStoreGroup tblStoreGroup = db.tblStoreGroups.Find(id);
            if (tblStoreGroup == null)
            {
                return HttpNotFound();
            }
            return View(tblStoreGroup);
        }

        // GET: StoreGroups/Create
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

        // POST: StoreGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StoreGroupID,Name,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblStoreGroup tblStoreGroup)
        {
         
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                if (ModelState.IsValid)
                {
                    LogInfo.Comments = @MujiStore.Resources.Resource.StoreGroupCreateCommon + tblStoreGroup.Name.Trim();

                    tblStoreGroup.CRTDT = DateTime.Now;
                    tblStoreGroup.CRTCD = Session["UserName"].ToString();
                    tblStoreGroup.IPAddress = Session["IPAddress"].ToString();
                    tblStoreGroup.DELFG = false;
                    //tblVideoDemoStoreMst.UdDate = DateTime.Now;
                    db.tblStoreGroups.Add(tblStoreGroup);
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntStoreGroupsCreateSuccMsg;
                    return RedirectToAction("Index");
                }
               
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupsCreateErrMsg1;
                return View(tblStoreGroup);
            }
            catch (Exception ex)
            {
                
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupsCreateErrMsg2;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupsCreateErrMsg3;
                }
                
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: StoreGroups/Edit/5
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
                tblStoreGroup tblStoregroup = db.tblStoreGroups.Find(id);
                
                if (tblStoregroup == null)
                {
                    return HttpNotFound();
                }
                return View(tblStoregroup);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: StoreGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StoreGroupID,Name,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblStoreGroup tblStoreGroup)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                if (ModelState.IsValid)
                {

                    LogInfo.Comments = @MujiStore.Resources.Resource.StoreGroupCreateCommon + tblStoreGroup.Name.Trim();

                    tblStoreGroup.UPDDT = DateTime.Now;
                    tblStoreGroup.UPDCD = Session["UserName"].ToString();
                    tblStoreGroup.IPAddress = Session["IPAddress"].ToString();
                    db.Entry(tblStoreGroup).State = EntityState.Modified;
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntStoreGroupsEditSuccMsg;
                    return RedirectToAction("Index");
                }
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupsEditErrMsg1;
                return View(tblStoreGroup);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupsEditErrMsg2;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreGroupsEditErrMsg3;
                }
               
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        [NonAction]
        // GET: StoreGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStoreGroup tblStoreGroup = db.tblStoreGroups.Find(id);
            if (tblStoreGroup == null)
            {
                return HttpNotFound();
            }
            return View(tblStoreGroup);
        }
        [NonAction]
        // POST: StoreGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblStoreGroup tblStoreGroup = db.tblStoreGroups.Find(id);
            db.tblStoreGroups.Remove(tblStoreGroup);
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
