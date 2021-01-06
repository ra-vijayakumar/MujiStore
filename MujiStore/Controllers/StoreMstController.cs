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
    public class StoreMstController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: StoreMst
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
                ViewData["StoreInfo"] = db.tblStores.Include(t => t.tblStoreGroup).ToList().OrderByDescending(x => x.StoreID).ToPagedList(pageNumber, pageSize);
                return View();
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
            // test comments on 2021-01-05 
            // test comments on 2021-01-05 _03
        }

        // GET: StoreMst/Details/5
        [NonAction]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStore tblStore = db.tblStores.Find(id);
            if (tblStore == null)
            {
                return HttpNotFound();
            }
            return View(tblStore);
        }

        // GET: StoreMst/Create
        public ActionResult Create()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                BindStoreGroup(0);
                return View();
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
           
        }

        public void BindStoreGroup(int? val)
        {
           
            List<SelectListItem> items = new SelectList(db.tblStoreGroups.Where(x => x.DELFG == false), "StoreGroupID", "Name").ToList();
           // items.Insert(0, (new SelectListItem { Text = "Select", Value = null }));

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select"
            });
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

            ViewBag.StoreGroupID = selectList;
           
 
        }
        

        // POST: StoreMst/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StoreID,StoreName,AddressLine1,AddressLine2,City,State,Zip,Country,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress,StoreIPAddress,StoreGroupID")] tblStore tblVideoDemoStoreMst)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;

            try
            {
                ModelState["StoreGroupID"].Errors.Clear();
                BindStoreGroup(tblVideoDemoStoreMst.StoreGroupID);
                if (ModelState.IsValid)
                {


                    var sname = db.tblStores.Where(x => x.StoreName.ToLower().Trim().ToString() == tblVideoDemoStoreMst.StoreName.ToLower().ToString()).FirstOrDefault();
                    if (sname != null)
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreMstCreateErrMsg1;
                        return View(tblVideoDemoStoreMst);
                    }

                    LogInfo.Comments = "Store Created - " + tblVideoDemoStoreMst.StoreName.Trim();
                    if(tblVideoDemoStoreMst.StoreGroupID == 0)
                    {
                        tblVideoDemoStoreMst.StoreGroupID = null;
                    }
                    tblVideoDemoStoreMst.CRTDT = DateTime.Now;
                    tblVideoDemoStoreMst.CRTCD = Session["UserName"].ToString();
                    tblVideoDemoStoreMst.IPAddress = Session["IPAddress"].ToString();
                    tblVideoDemoStoreMst.DELFG = false;
                    db.tblStores.Add(tblVideoDemoStoreMst);
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntStoreMstCreateSuccMsg;
                    return RedirectToAction("Index");
                }
                if (tblVideoDemoStoreMst.StoreGroupID == null)
                {
                    tblVideoDemoStoreMst.StoreGroupID = 0;
                }
                
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreMstCreateErrMsg2;
                return View(tblVideoDemoStoreMst);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreMstCreateErrMsg3;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreMstCreateErrMsg2;
                }
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreMstCreateErrMsg2;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

            
        }

        // GET: StoreMst/Edit/5
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
                tblStore tblStoreMst = db.tblStores.Find(id);
                BindStoreGroup(tblStoreMst.StoreGroupID);
                if (tblStoreMst == null)
                {
                    return HttpNotFound();
                }
                return View(tblStoreMst);
            }
            catch(Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
            
            //return View(tblStore);
        }

        // POST: StoreMst/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StoreID,StoreName,AddressLine1,AddressLine2,City,State,Zip,Country,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress,StoreIPAddress,StoreGroupID")] tblStore tblStore)
        {
   
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                BindStoreGroup(tblStore.StoreGroupID);

                var sname = db.tblStores.Where(x => x.StoreName.ToLower().Trim().ToString() == tblStore.StoreName.ToLower().ToString() && x.StoreID != tblStore.StoreID).FirstOrDefault();
                if (sname != null)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreMstEditErrMsg1;
                    return View(tblStore);
                }
                //ModelState["StoreGroupID"].Errors.Clear();
                //ViewBag.StoreGroupID = new SelectList(db.tblStoreGroups, "StoreGroupID", "Name", tblStore.StoreGroupID);
                if (ModelState.IsValid)
                {
                    
                    LogInfo.Comments = "Store Updated - " + tblStore.StoreName.Trim();

                    if (tblStore.StoreGroupID == 0)
                    {
                        tblStore.StoreGroupID = null;
                    }
                    tblStore.UPDDT = DateTime.Now;
                    tblStore.UPDCD = Session["UserName"].ToString();
                    tblStore.IPAddress = Session["IPAddress"].ToString();
                    db.Entry(tblStore).State = EntityState.Modified;
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntStoreMstEditSuccMsg;
                    return RedirectToAction("Index");
                }
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreMstEditErrMsg2;
                return View(tblStore);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                 if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreMstEditErrMsg3;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntStoreMstEditErrMsg2;
                }
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        [NonAction]
        // GET: StoreMst/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStore tblStore = db.tblStores.Find(id);
            if (tblStore == null)
            {
                return HttpNotFound();
            }
            return View(tblStore);
        }
        [NonAction]
        // POST: StoreMst/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblStore tblStore = db.tblStores.Find(id);
            db.tblStores.Remove(tblStore);
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
