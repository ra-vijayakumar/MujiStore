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
    public class RobocopyExitcodesController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: RobocopyExitcodes
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
                return View(db.tblRobocopyExitcodes.ToList().OrderByDescending(x => x.RobocopyExitcodeID).ToPagedList(pageNumber, pageSize));
                //return View(db.tblFormats.ToList().OrderByDescending(x => x.FormatID).ToPagedList(pageNumber, pageSize));

            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: RobocopyExitcodes/Details/5
        [NonAction]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblRobocopyExitcode tblRobocopyExitcode = db.tblRobocopyExitcodes.Find(id);
            if (tblRobocopyExitcode == null)
            {
                return HttpNotFound();
            }
            return View(tblRobocopyExitcode);
        }

        // GET: RobocopyExitcodes/Create

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

        // POST: RobocopyExitcodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RobocopyExitcodeID,Content,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblRobocopyExitcode tblRobocopyExitcode)
        {
         
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;



            try
            {
                var sname = db.tblRobocopyExitcodes.Where(x => x.Content.ToLower().Trim().ToString() == tblRobocopyExitcode.Content.ToLower().ToString()).FirstOrDefault();
                if (sname != null)
                {
                    TempData["ErrMsg"] = @MujiStore.Resources.Resource.UnableCreateRoboCopyExistsCodes;
                    return View(tblRobocopyExitcode);
                }

                if (ModelState.IsValid)
                {

                    LogInfo.Comments = @MujiStore.Resources.Resource.RoboCopyExistsCodescreated + tblRobocopyExitcode.Content.ToString();


                    tblRobocopyExitcode.CRTDT = DateTime.Now;
                    tblRobocopyExitcode.CRTCD = Session["UserName"].ToString();
                    tblRobocopyExitcode.IPAddress = Session["IPAddress"].ToString();
                    db.tblRobocopyExitcodes.Add(tblRobocopyExitcode);
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = @MujiStore.Resources.Resource.FormatCreatedSuccessfully;
                    return RedirectToAction("Index");
                }

                return View(tblRobocopyExitcode);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = @MujiStore.Resources.Resource.UnableCreateRoboCopyExistsCodes1;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: RobocopyExitcodes/Edit/5
        public ActionResult Edit(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //tblRobocopyExitcode tblRobocopyExitcode = db.tblRobocopyExitcodes.Find(id);
            //if (tblRobocopyExitcode == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(tblRobocopyExitcode);
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblRobocopyExitcode robocode = db.tblRobocopyExitcodes.Find(id);
                if (robocode == null)
                {
                    return HttpNotFound();
                }

                return View(robocode);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: RobocopyExitcodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RobocopyExitcodeID,Content,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblRobocopyExitcode tblRobocopyExitcode)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Entry(tblRobocopyExitcode).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //return View(tblRobocopyExitcode);
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {


                var robocode = db.tblRobocopyExitcodes.Where(x => x.Content.ToLower().Trim().ToString() == tblRobocopyExitcode.Content.ToLower().ToString() && x.RobocopyExitcodeID != tblRobocopyExitcode.RobocopyExitcodeID).FirstOrDefault();
                if (robocode != null)
                {
                    TempData["ErrMsg"] = @MujiStore.Resources.Resource.UnableRoboCopyExistsCodesRoboCopyExistsCodesAlreadyexists;
                    return View(tblRobocopyExitcode);
                }


                if (ModelState.IsValid)
                {

                    LogInfo.Comments = @MujiStore.Resources.Resource.RoboCopyExistsCodesUpdated + tblRobocopyExitcode.Content.ToString();
                    tblRobocopyExitcode.UPDDT = DateTime.Now;
                    tblRobocopyExitcode.UPDCD = Session["UserName"].ToString();
                    tblRobocopyExitcode.IPAddress = Session["IPAddress"].ToString();
                    db.Entry(tblRobocopyExitcode).State = EntityState.Modified;
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = @MujiStore.Resources.Resource.RoboCopyExitCodesUpdatedSuccessfully;
                    return RedirectToAction("Index");
                }

                TempData["ErrMsg"] = @MujiStore.Resources.Resource.UnableUpdateRoboCopyExistsCodes;
                return View(tblRobocopyExitcode);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = @MujiStore.Resources.Resource.UnableUpdateRoboCopyExistsCodes;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: RobocopyExitcodes/Delete/5
        [NonAction]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblRobocopyExitcode tblRobocopyExitcode = db.tblRobocopyExitcodes.Find(id);
            if (tblRobocopyExitcode == null)
            {
                return HttpNotFound();
            }
            return View(tblRobocopyExitcode);
        }

        // POST: RobocopyExitcodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NonAction]
        public ActionResult DeleteConfirmed(int id)
        {
            tblRobocopyExitcode tblRobocopyExitcode = db.tblRobocopyExitcodes.Find(id);
            db.tblRobocopyExitcodes.Remove(tblRobocopyExitcode);
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
