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
    public class StoreSubnetsController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: StoreSubnets
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
                return View(db.tblStoreSubnets.Include(t => t.tblStore).Include(t => t.tblSubnet).ToList().OrderByDescending(x => x.StoreSubnetID).ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        [NonAction]
        // GET: StoreSubnets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStoreSubnet tblStoreSubnet = db.tblStoreSubnets.Find(id);
            if (tblStoreSubnet == null)
            {
                return HttpNotFound();
            }
            return View(tblStoreSubnet);
        }

        // GET: StoreSubnets/Create
        public ActionResult Create()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                BindStore(0);
                BindSubnet(0);
                return View();
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
         
        }

        // POST: StoreSubnets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StoreSubnetID,Store,Subnet,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblStoreSubnet tblStoreSubnet)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;

            try
            {
                BindStore(tblStoreSubnet.Store);
                BindSubnet(tblStoreSubnet.Subnet);
                //ModelState["StoreGroupID"].Errors.Clear();
                if (tblStoreSubnet.Store == 0)
                {
                   // ModelState["Store"] = true;
                    ModelState.AddModelError("Store", "Select Store");
                    
                }
                if (tblStoreSubnet.Subnet == 0)
                {
                    ModelState.AddModelError("Subnet", "Select Subnet");
                }
               
                if (ModelState.IsValid)
                {

                    //var tblstoresubnetid = db.tblStoreSubnets.Where(x => x.Subnet == tblStoreSubnet.Subnet && x.Store == tblStoreSubnet.Store).FirstOrDefault();
                    var tblstoreid = db.tblStoreSubnets.Where(x => x.Store == tblStoreSubnet.Store && x.DELFG == false).FirstOrDefault();
                    if (tblstoreid != null)
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsCreateErrMsg1;
                        return View(tblStoreSubnet);
                    }
                    var tblsubnetid = db.tblStoreSubnets.Where(x => x.Subnet == tblStoreSubnet.Subnet && x.DELFG == false).FirstOrDefault();
                    if (tblsubnetid != null)
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsCreateErrMsg1;
                        return View(tblStoreSubnet);
                    }
                    LogInfo.Comments = "Store Subnet Created - " + tblStoreSubnet.Store+ "," + tblStoreSubnet.tblSubnet;

                    tblStoreSubnet.CRTDT = DateTime.Now;
                    tblStoreSubnet.CRTCD = Session["UserName"].ToString();
                    tblStoreSubnet.IPAddress = Session["IPAddress"].ToString();
                    tblStoreSubnet.DELFG = false;
                    db.tblStoreSubnets.Add(tblStoreSubnet);
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsCreateSuccMsg;
                    return RedirectToAction("Index");
                }
                //BindStoreGroup();
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsCreateErrMsg2;

                BindStore(tblStoreSubnet.Store);
                BindSubnet(tblStoreSubnet.Subnet);
                return View(tblStoreSubnet);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsCreateErrMsg3;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsCreateErrMsg2;
                }
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsCreateErrMsg2;
                BindStore(tblStoreSubnet.Store);
                BindSubnet(tblStoreSubnet.Subnet);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

     
        }

        // GET: StoreSubnets/Edit/5
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
                tblStoreSubnet tblStoreSubnet = db.tblStoreSubnets.Find(id);

                if (tblStoreSubnet == null)
                {
                    return HttpNotFound();
                }
                BindStore(tblStoreSubnet.Store);
                BindSubnet(tblStoreSubnet.Subnet);
             return View(tblStoreSubnet);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        } 

        // POST: StoreSubnets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StoreSubnetID,Store,Subnet,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblStoreSubnet tblStoreSubnet)
        {

            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                BindStore(tblStoreSubnet.Store);
                BindSubnet(tblStoreSubnet.Subnet);

                if (tblStoreSubnet.Store == 0)
                {
                    ModelState.AddModelError("Store", "Select Store");

                }
                if (tblStoreSubnet.Subnet == 0)
                {
                    ModelState.AddModelError("Subnet", "Select Subnet");
                }
                if (ModelState.IsValid)
                {

                    LogInfo.Comments = "Store Subnet Updated - " + tblStoreSubnet.Store + "," + tblStoreSubnet.Subnet;
                    if (tblStoreSubnet.DELFG == false)
                    {
                        //db.tblFolders.Where(x => x.FolderID != 1).ToList().OrderByDescending(x => x.FolderID).ToPagedList(pageNumber, pageSize);
                        var tblstoreid = db.tblStoreSubnets.Where(x => x.StoreSubnetID != tblStoreSubnet.StoreSubnetID && x.Store == tblStoreSubnet.Store && x.DELFG == false).FirstOrDefault();

                        if (tblstoreid != null)
                        {
                            TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsEditErrMsg1;
                            return View(tblStoreSubnet);
                        }

                        var tblsubnetid = db.tblStoreSubnets.Where(x => x.StoreSubnetID != tblStoreSubnet.StoreSubnetID && x.Subnet == tblStoreSubnet.Subnet && x.DELFG == false).FirstOrDefault();

                        if (tblsubnetid != null)
                        {
                            TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsEditErrMsg1;
                            return View(tblStoreSubnet);
                        }
                        
                            LogInfo.Comments = "Subnet Updated - Store ID : "  +tblStoreSubnet.Store + " ,Subnet ID : " + tblStoreSubnet.Subnet;

                            tblStoreSubnet.UPDDT = DateTime.Now;
                            tblStoreSubnet.UPDCD = Session["UserName"].ToString();
                            tblStoreSubnet.IPAddress = Session["IPAddress"].ToString();
                            db.Entry(tblStoreSubnet).State = EntityState.Modified;
                            db.SaveChanges();
                            CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                            TempData["SuccMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsEditSuccMsg;
                            return RedirectToAction("Index");
                      
                    }
                    else
                    {
                        tblStoreSubnet.UPDDT = DateTime.Now;
                        tblStoreSubnet.UPDCD = Session["UserName"].ToString();
                        tblStoreSubnet.IPAddress = Session["IPAddress"].ToString();
                        db.Entry(tblStoreSubnet).State = EntityState.Modified;
                        db.SaveChanges();
                        CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                        TempData["SuccMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsEditSuccMsg;
                        return RedirectToAction("Index");
                    }
                    
                }
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsEditErrMsg2;
                BindStore(tblStoreSubnet.Store);
                BindSubnet(tblStoreSubnet.Subnet);
                return View(tblStoreSubnet);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                //TempData["ErrMsg"] = "Unable Update Store";
                if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsEditErrMsg3;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreSubnetsEditErrMsg2;
                }
                BindStore(tblStoreSubnet.Store);
                BindSubnet(tblStoreSubnet.Subnet);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }


        public void BindStore(int val)
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select"
            });
            List<SelectListItem> items = new SelectList(db.tblStores.Where(x => x.DELFG == false), "StoreID", "StoreName").ToList();
            foreach (var element in items)
            {
                if(val == Convert.ToInt32(element.Value))
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = element.Value,
                        Text = element.Text,
                        Selected = true
                        
                    });
                }
                else
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = element.Value,
                        Text = element.Text
                    });

                }
               
            }

           ViewBag.Store = selectList;
        }

        
        public void BindSubnet(int val)
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select"
            });

            List<SelectListItem> items = new SelectList(db.tblSubnets.Where(x => x.DELFG == false), "SubnetID", "SNIPAddress").ToList();
            foreach (var element in items)
            {
                if (val == Convert.ToInt32(element.Value))
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = element.Value,
                        Text = element.Text,
                        Selected = true

                    });
                }
                else
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = element.Value,
                        Text = element.Text
                    });

                }

            }
        
            ViewBag.Subnet = selectList;
        }
        [NonAction]
        // GET: StoreSubnets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStoreSubnet tblStoreSubnet = db.tblStoreSubnets.Find(id);
            if (tblStoreSubnet == null)
            {
                return HttpNotFound();
            }
            return View(tblStoreSubnet);
        }
        [NonAction]
        // POST: StoreSubnets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblStoreSubnet tblStoreSubnet = db.tblStoreSubnets.Find(id);
            db.tblStoreSubnets.Remove(tblStoreSubnet);
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
