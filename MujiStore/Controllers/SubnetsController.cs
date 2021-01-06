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
    public class SubnetsController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: Subnets
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
               
                return View(db.tblSubnets.ToList().OrderByDescending(x => x.SubnetID).ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        public ActionResult TroubleShoot()
        {
            return View();
        }
        public ActionResult TroubleShoot(tblSubnet tblSubnet)
        {
            if (ModelState.IsValid)
            {
            }
                return View();
        }
        // GET: Subnets/Details/5
        [NonAction]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSubnet tblSubnet = db.tblSubnets.Find(id);
            if (tblSubnet == null)
            {
                return HttpNotFound();
            }
            return View(tblSubnet);
        }

        // GET: Subnets/Create
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
            //return View();
        }

        // POST: Subnets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubnetID,SNIPAddress,SubnetMask,WANBandWidth,LANBandWidth,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblSubnet tblSubnet)
        {
             LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                if (ModelState.IsValid)
                {

                    List<tblSubnet> tblsubnetid = db.tblSubnets.Where(x => x.SNIPAddress == tblSubnet.SNIPAddress && x.DELFG == false).ToList();

                    if (tblsubnetid.Count() > 0)
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CntSubnetsCreateErrMsg1;
                        return View(tblSubnet);
                    }

                    LogInfo.Comments = "Subnet Created - " + tblSubnet.SNIPAddress.Trim();
                    int GSubnetID = 0;
                    GSubnetID =  GettblSubnetID();
                    tblSubnet.SubnetID = GSubnetID;
                    tblSubnet.CRTDT = DateTime.Now;
                    tblSubnet.CRTCD = Session["UserName"].ToString();
                    tblSubnet.IPAddress = Session["IPAddress"].ToString();
                    tblSubnet.DELFG = false;
                    db.tblSubnets.Add(tblSubnet);
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntSubnetsCreateSuccMsg;
                    return RedirectToAction("Index");
                }
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntSubnetsCreateErrMsg2;
                return View(tblSubnet);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntSubnetsCreateErrMsg3;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntSubnetsCreateErrMsg2;
                }
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntSubnetsCreateErrMsg2;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }

        public int GettblSubnetID()
        {
            int id = 0;
            id = db.tblSubnets.Select(p => p.SubnetID).DefaultIfEmpty(0).Max();
           
            id = id + 1;
            return id;
        }
        // GET: Subnets/Edit/5
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
                tblSubnet tblsubnetid = db.tblSubnets.Find(id);
                if (tblsubnetid == null)
                {
                    return HttpNotFound();
                }
                return View(tblsubnetid);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }

        // POST: Subnets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SubnetID,SNIPAddress,SubnetMask,WANBandWidth,LANBandWidth,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblSubnet tblSubnet)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                if (ModelState.IsValid)
                {
                            if (tblSubnet.DELFG == false)
                            {
                                List<tblSubnet> tblsubnetid = db.tblSubnets.Where(x => x.SubnetID != tblSubnet.SubnetID && x.SNIPAddress == tblSubnet.SNIPAddress && x.DELFG == false).ToList();

                                if( tblsubnetid.Count()> 0 )
                                {
                                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntSubnetsEditErrMsg1;
                                    return View(tblSubnet);
                                }
                                else
                                {
                                    LogInfo.Comments = "Subnet Updated - " + tblSubnet.SNIPAddress.Trim();

                                    tblSubnet.UPDDT = DateTime.Now;
                                    tblSubnet.UPDCD = Session["UserName"].ToString();
                                    tblSubnet.IPAddress = Session["IPAddress"].ToString();
                                    db.Entry(tblSubnet).State = EntityState.Modified;
                                    db.SaveChanges();
                                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntSubnetsEditSuccMsg;
                                    return RedirectToAction("Index");
                                }
                            }
                            else
                            {
                                LogInfo.Comments = "Subnet Updated - " + tblSubnet.SNIPAddress.Trim();

                                tblSubnet.UPDDT = DateTime.Now;
                                tblSubnet.UPDCD = Session["UserName"].ToString();
                                tblSubnet.IPAddress = Session["IPAddress"].ToString();
                                db.Entry(tblSubnet).State = EntityState.Modified;
                                db.SaveChanges();
                                CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                                TempData["SuccMsg"] = MujiStore.Resources.Resource.CntSubnetsEditSuccMsg;
                                return RedirectToAction("Index");
                            }
       
                }
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntSubnetsEditErrMsg2;
                return View(tblSubnet);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntSubnetsEditErrMsg3;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntSubnetsEditErrMsg2;
                }
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        [NonAction]
        // GET: Subnets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSubnet tblSubnet = db.tblSubnets.Find(id);
            if (tblSubnet == null)
            {
                return HttpNotFound();
            }
            return View(tblSubnet);
        }
        [NonAction]
        // POST: Subnets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblSubnet tblSubnet = db.tblSubnets.Find(id);
            db.tblSubnets.Remove(tblSubnet);
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
