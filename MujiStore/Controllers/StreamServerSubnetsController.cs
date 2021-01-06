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
    public class StreamServerSubnetsController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: StreamServerSubnets
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
                //var tblStreamServerSubnets = db.tblStreamServerSubnets.Include(t => t.tblSubnet);
                //return View(tblStreamServerSubnets.ToList());
                return View(db.tblStreamServerSubnets.Include(t => t.tblSubnet).ToList().OrderByDescending(x => x.StreamServerSubnetID).ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: StreamServerSubnets/Details/5
        [NonAction]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStreamServerSubnet tblStreamServerSubnet = db.tblStreamServerSubnets.Find(id);
            if (tblStreamServerSubnet == null)
            {
                return HttpNotFound();
            }
            return View(tblStreamServerSubnet);
        }

        // GET: StreamServerSubnets/Create
        public ActionResult Create()
        {
            

            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                ViewBag.Subnet = new SelectList(db.tblSubnets.Where(x => x.DELFG == false), "SubnetID", "SNIPAddress");
                return View();

            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: StreamServerSubnets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StreamServerSubnetID,SSSServer,Subnet,FormatID,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblStreamServerSubnet tblStreamServerSubnet)
        {
            //if (ModelState.IsValid)
            //{
            //    db.tblStreamServerSubnets.Add(tblStreamServerSubnet);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            //ViewBag.Subnet = new SelectList(db.tblSubnets.Where(x => x.DELFG == false), "SubnetID", "SNIPAddress", tblStreamServerSubnet.Subnet);
            //return View(tblStreamServerSubnet);
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;

            try
            {
                ViewBag.Subnet = new SelectList(db.tblSubnets.Where(x => x.DELFG == false), "SubnetID", "SNIPAddress",tblStreamServerSubnet.Subnet);
              
                //ModelState["StoreGroupID"].Errors.Clear();
                //if (tblStreamServerSubnet.Subnet == null)
                //{
                //    // ModelState["Store"] = true;
                //    ModelState.AddModelError("Subnet", "Select Subnet");

                //}
               

                if (ModelState.IsValid)
                {

                    //var tblstoresubnetid = db.tblStoreSubnets.Where(x => x.Subnet == tblStoreSubnet.Subnet && x.Store == tblStoreSubnet.Store).FirstOrDefault();
                    var tblsubnetid = db.tblStreamServerSubnets.Where(x => x.Subnet == tblStreamServerSubnet.Subnet && x.DELFG == false).FirstOrDefault();
                    if (tblsubnetid != null)
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonCreateSubnetAlreadyExists;
                        return View(tblStreamServerSubnet);
                    }
                    var tblSSSServer = db.tblStreamServerSubnets.Where(x => x.SSSServer == tblStreamServerSubnet.SSSServer && x.DELFG == false).FirstOrDefault();
                    if (tblSSSServer != null)
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonCreateStreamServerSubnetServerAlExists;
                        return View(tblStreamServerSubnet);
                    }
                    LogInfo.Comments = "Stream Server Subnet Created - " + tblStreamServerSubnet.SSSServer + "," + tblStreamServerSubnet.Subnet;

                    tblStreamServerSubnet.CRTDT = DateTime.Now;
                    tblStreamServerSubnet.CRTCD = Session["UserName"].ToString();
                    tblStreamServerSubnet.IPAddress = Session["IPAddress"].ToString();
                    db.tblStreamServerSubnets.Add(tblStreamServerSubnet);
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CommonNmeStreamServerSubnetCreatedSuccess;
                    return RedirectToAction("Index");
                }
                //BindStoreGroup();
                if (ModelState.IsValid)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonNameUnableCreateStreamSubnet;
                }
                    

                return View(tblStreamServerSubnet);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
             
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonNameUnableCreateStreamSubnet;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: StreamServerSubnets/Edit/5
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
                tblStreamServerSubnet tblStreamServerSubnet = db.tblStreamServerSubnets.Find(id);

                if (tblStreamServerSubnet == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Subnet = new SelectList(db.tblSubnets.Where(x => x.DELFG == false), "SubnetID", "SNIPAddress", tblStreamServerSubnet.Subnet);
                return View(tblStreamServerSubnet);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: StreamServerSubnets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StreamServerSubnetID,SSSServer,Subnet,FormatID,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblStreamServerSubnet tblStreamServerSubnet)
        {
           
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                ViewBag.Subnet = new SelectList(db.tblSubnets.Where(x => x.DELFG == false), "SubnetID", "SNIPAddress", tblStreamServerSubnet.Subnet);
                if (ModelState.IsValid)
                {

                    //var tblstoresubnetid = db.tblStoreSubnets.Where(x => x.Subnet == tblStoreSubnet.Subnet && x.Store == tblStoreSubnet.Store).FirstOrDefault();
                    var tblsubnetid = db.tblStreamServerSubnets.Where(x => x.Subnet == tblStreamServerSubnet.Subnet && x.StreamServerSubnetID != tblStreamServerSubnet.StreamServerSubnetID  && x.DELFG == false).FirstOrDefault();
                    if (tblsubnetid != null)
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonCreateSubnetAlreadyExists;
                        return View(tblStreamServerSubnet);
                    }
                    var tblSSSServer = db.tblStreamServerSubnets.Where(x => x.SSSServer == tblStreamServerSubnet.SSSServer && x.StreamServerSubnetID != tblStreamServerSubnet.StreamServerSubnetID  && x.DELFG == false).FirstOrDefault();
                    if (tblSSSServer != null)
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonCreateStreamServerSubnetServerAlExists;
                        return View(tblStreamServerSubnet);
                    }
                    LogInfo.Comments = "Stream Server Subnet Updated - " + tblStreamServerSubnet.SSSServer + "," + tblStreamServerSubnet.Subnet;

                    tblStreamServerSubnet.UPDDT = DateTime.Now;
                    tblStreamServerSubnet.UPDCD = Session["UserName"].ToString();
                    tblStreamServerSubnet.IPAddress = Session["IPAddress"].ToString();
                    db.Entry(tblStreamServerSubnet).State = EntityState.Modified;
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CommonCreateSubnetUpdatedSuccess;
                    return RedirectToAction("Index");
                }
                //BindStoreGroup();
                if (ModelState.IsValid)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonCreateUnableUpdateSubnet;
                }


                return View(tblStreamServerSubnet);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                //TempData["ErrMsg"] = "Unable Update Store";
                
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: StreamServerSubnets/Delete/5
        [NonAction]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStreamServerSubnet tblStreamServerSubnet = db.tblStreamServerSubnets.Find(id);
            if (tblStreamServerSubnet == null)
            {
                return HttpNotFound();
            }
            return View(tblStreamServerSubnet);
        }

        // POST: StreamServerSubnets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NonAction]
        public ActionResult DeleteConfirmed(int id)
        {
            tblStreamServerSubnet tblStreamServerSubnet = db.tblStreamServerSubnets.Find(id);
            db.tblStreamServerSubnets.Remove(tblStreamServerSubnet);
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
