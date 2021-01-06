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

using System.Configuration;
using System.Data.SqlClient;


namespace MujiStore.Controllers
{
    [SessionExpire]
    [Authorize]
    //[MUJICustomAuthorize(Roles = "A,U")]
    [Authorize(Roles = "16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31")]
    public class UserController : Controller
    {
         private mujiEntities1 db = new mujiEntities1();

        // GET: User
        public ActionResult Index(int? page)
        {
            
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            int pageSize;
            if (System.Configuration.ConfigurationManager.AppSettings["PageSize"]== null)
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
                //PopulateRole();
                ViewData["RoleDtl"] = BindRole();

                ViewData["UserInfo"] = db.tblUsers.ToList().OrderByDescending(x => x.UserID).ToPagedList(pageNumber, pageSize);
                return View();
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

   
        // GET: User/Create
        public ActionResult Create()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                ModelState.Clear();
                return View();
               }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName)); ;
            }

        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,UserName,UserEmail,Password,CreateUserId,CreateDate,UpdateUserId,UpdateDate,DELFG,Authority1,Authority2,Authority4,Authority8,Authority16")] tblUser tbl_User,string ConfirmPassword)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            //user_tbl tbl_User = new user_tbl();
            
            tbl_User.Authority = 0;
            try
            {
                ModelState["OldPassword"].Errors.Clear();
                ModelState["NewPassword"].Errors.Clear();
                ModelState["ConfirmNewPassword"].Errors.Clear();
                // PopulateRole();
                if (ModelState.IsValid)
                {
                    if (tbl_User.Password.ToString().Trim() != ConfirmPassword.Trim())
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CntUserCreateErrMsg1;
                        return View(tbl_User);
                    }

                    if (tbl_User.Authority1 == true)
                    {
                        tbl_User.Authority += 1;
                    }
                    if (tbl_User.Authority2 == true)
                    {
                        tbl_User.Authority += 2;
                    }
                    if (tbl_User.Authority4 == true)
                    {
                        tbl_User.Authority += 4;
                    }
                    if (tbl_User.Authority8 == true)
                    {
                        tbl_User.Authority += 8;
                    }
                    if (tbl_User.Authority16 == true)
                    {
                        tbl_User.Authority += 16;
                    }
                    tbl_User.CRTDT = DateTime.Now;
                    tbl_User.Role = "U";
                    tbl_User.CRTCD = Session["UserName"].ToString();
                    tbl_User.IPAddress = Session["IPAddress"].ToString();
                    db.tblUsers.Add(tbl_User);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;
                    LogInfo.Comments = "User Created - " + tbl_User.UserName.ToString();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntUserCreateSuccMsg;
                    return RedirectToAction("Index");
               }

                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntUserCreateErrMsg2;
                return View(tbl_User);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntUserCreateErrMsg3;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntUserCreateErrMsg2;
                }
                
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                //PopulateRole();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblUser tbl_User = db.tblUsers.Find(id);
                if (tbl_User == null)
                {
                    return HttpNotFound();
                }
                tbl_User.OldPassword = "test";
                tbl_User.NewPassword = "test";
                tbl_User.ConfirmNewPassword = "test";

                return View(tbl_User);
            }

            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }
        
        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,UserName,UserEmail,Password,CreateUserId,CRTCD,CRTDT,DELFG,Authority1,Authority2,Authority4,Authority8,Authority16,OldPassword,NewPassword,ConfirmNewPassword")] tblUser tbl_User,string ConfirmPassword)
        {
        
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            tbl_User.Authority = 0;
            try
            {
                if (ModelState.IsValid) 
                {
                    if (tbl_User.Password.ToString().Trim() != ConfirmPassword.Trim())
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CntUserEditErrMsg1;
                        return RedirectToAction("Edit",new {id = tbl_User.UserID } );
                    }
                    if (tbl_User.Authority1 == true)
                    {
                        tbl_User.Authority += 1;
                    }
                    if (tbl_User.Authority2 == true)
                    {
                        tbl_User.Authority += 2;
                    }
                    if (tbl_User.Authority4 == true)
                    {
                        tbl_User.Authority += 4;
                    }
                    if (tbl_User.Authority8 == true)
                    {
                        tbl_User.Authority += 8;
                    }
                    if (tbl_User.Authority16 == true)
                    {
                        tbl_User.Authority += 16;
                    }
                    tbl_User.Role = "U";
                    tbl_User.UPDDT = DateTime.Now;
                    tbl_User.UPDCD = Session["UserName"].ToString();
                    tbl_User.IPAddress = Session["IPAddress"].ToString();
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Entry(tbl_User).State = EntityState.Modified;
                    db.Configuration.ValidateOnSaveEnabled = true;
                    db.SaveChanges();
                    LogInfo.Comments = "User Updated - " + tbl_User.UserName.ToString();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntUserEditSuccMsg;
                    return RedirectToAction("Index");
                }
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntUserEditErrMsg2;
                return View(tbl_User);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                if (((System.Data.SqlClient.SqlException)ex.InnerException.InnerException).Number == 2627)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntUserEditErrMsg3;
                }
                else
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CntUserEditErrMsg2;
                }
                
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //begins
        public void PopulateRole()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("Value");
            _dt.Columns.Add("Text");
            DataRow row = _dt.NewRow();
            row["Value"] = "U";
            row["Text"] = "User";
            _dt.Rows.Add(row);
            row = _dt.NewRow();
            row["Value"] = "A";
            row["Text"] = "Admin";
            _dt.Rows.Add(row);
            row = _dt.NewRow();

            ViewBag.RoleList = CommonLogic.ToSelectList(_dt, "Value", "Text");
        }
        public List<SelectListItem> BindRole()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "User", Value = "U" });
            list.Add(new SelectListItem { Text = "Admin", Value = "A" });
            return list;
        }
            //endshere
        }
}
