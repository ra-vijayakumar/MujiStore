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
    public class DeploySchedulesController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: DeploySchedules
        public ActionResult Index(int? page)
        {
            //return View(db.tblDeploySchedules.ToList());
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
                return View(db.tblDeploySchedules.ToList().OrderByDescending(x => x.DeployScheduleID).ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: DeploySchedules/Details/5
        [NonAction]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDeploySchedule tblDeploySchedule = db.tblDeploySchedules.Find(id);
            if (tblDeploySchedule == null)
            {
                return HttpNotFound();
            }
            return View(tblDeploySchedule);
        }

        // GET: DeploySchedules/Create
        public ActionResult Create()
        {
            // return View();
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

        // POST: DeploySchedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeployScheduleID,Name,Schedule,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblDeploySchedule tblDeploySchedule)
        {

            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;



            try
            {
                var sname = db.tblDeploySchedules.Where(x => x.Name.ToLower().Trim().ToString() == tblDeploySchedule.Name.ToLower().ToString()
                && x.Schedule.ToLower().Trim().ToString() == tblDeploySchedule.Schedule.ToLower().ToString()).FirstOrDefault();
                if (sname != null)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.UnableCreateDeployeSch;
                    return View(tblDeploySchedule);
                }

                if (ModelState.IsValid)
                {

                    LogInfo.Comments = MujiStore.Resources.Resource.UnableCreateDeployeSchedule + tblDeploySchedule.Name.ToString() + "," + tblDeploySchedule.Schedule.ToString();

                    tblDeploySchedule.DELFG = false;
                    tblDeploySchedule.CRTDT = DateTime.Now;
                    tblDeploySchedule.CRTCD = Session["UserName"].ToString();
                    tblDeploySchedule.IPAddress = Session["IPAddress"].ToString();
                    db.tblDeploySchedules.Add(tblDeploySchedule);
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.UnableCreateDeployeScheduleCreateSuc;
                    return RedirectToAction("Index");
                }

                return View(tblDeploySchedule);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonNameUnableCreateDeploySchedule;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: DeploySchedules/Edit/5
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
                tblDeploySchedule deploysch = db.tblDeploySchedules.Find(id);
                if (deploysch == null)
                {
                    return HttpNotFound();
                }

                return View(deploysch);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: DeploySchedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeployScheduleID,Name,Schedule,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblDeploySchedule tblDeploySchedule)
        {
  
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {


                var depschname = db.tblDeploySchedules.Where(x => x.Name.ToLower().Trim().ToString() == tblDeploySchedule.Name.ToLower().ToString()
                && x.Schedule.ToLower().Trim().ToString() == tblDeploySchedule.Schedule.ToLower().ToString()
                && x.DeployScheduleID != tblDeploySchedule.DeployScheduleID).FirstOrDefault();

                if (depschname != null)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonNameAndScheduleAlreadyExist;
                    return View(tblDeploySchedule);
                }


                if (ModelState.IsValid)
                {

                   LogInfo.Comments = MujiStore.Resources.Resource.CommonNameDeployeScheduleUpdated + tblDeploySchedule.Name.ToString() + "," + tblDeploySchedule.Schedule.ToString();
                    tblDeploySchedule.UPDDT = DateTime.Now;
                    tblDeploySchedule.UPDCD = Session["UserName"].ToString();
                    tblDeploySchedule.IPAddress = Session["IPAddress"].ToString();
                    db.Entry(tblDeploySchedule).State = EntityState.Modified;
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CommonNameAndScheduleUpdateSuc;
                    return RedirectToAction("Index");
                }

                TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonNameAndScheduleUnableDeploySchedule;
                return View(tblDeploySchedule);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonNameAndScheduleUnableDeploySchedule;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: DeploySchedules/Delete/5
        [NonAction]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDeploySchedule tblDeploySchedule = db.tblDeploySchedules.Find(id);
            if (tblDeploySchedule == null)
            {
                return HttpNotFound();
            }
            return View(tblDeploySchedule);
        }

        // POST: DeploySchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NonAction]
        public ActionResult DeleteConfirmed(int id)
        {
            tblDeploySchedule tblDeploySchedule = db.tblDeploySchedules.Find(id);
            db.tblDeploySchedules.Remove(tblDeploySchedule);
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
