using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MujiStore.Models;
using System.Web.Security;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using MujiStore.BLL;
using System.Security.Principal;
using System.Threading;
using System.Data.Entity;
using System.Globalization;

namespace MujiStore.Controllers
{
    public class LoginController : Controller
    {
    
        string CS = ConfigurationManager.ConnectionStrings["cnstr"].ConnectionString;
        string query = "";
        private mujiEntities1 db = new mujiEntities1();
       
        // GET: /Security/Login/
        public ActionResult Index()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            CommonLogic.SetCultureInfo();
            try
            {
                CommonLogic.Log_info(LogInfo.MenuClick, "User Navigate Application");
                return View();
            }
            catch(Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        // test message 2020-01-06 03
        }

        [HttpPost]
        public ActionResult SignIn(tblUser user, string returnUrl)
        {
            try
            {

                LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
                LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
                CommonLogic.SetCultureInfo();
                if (Membership.ValidateUser(user.UserName, user.Password))
                {
                    tblUser tbluser = new tblUser();
                    Session["UserName"] = user.UserName;
                    Session["StoreUserName"] = user.UserName;
                   
                    tbluser = GetUserDetails(user.UserName);
                    Session["UserID"] = tbluser.UserID;
                    Session["UserEmail"] = tbluser.UserEmail;
                    Session["Role"] = tbluser.Authority;
                    if ((tbluser.Authority >= 4 && tbluser.Authority <= 7) ||
                        (tbluser.Authority >= 20 && tbluser.Authority <= 23))
                    {
                        Session["ApprovalFlag"] = "0";
                    }
                    else if ((tbluser.Authority >= 8 && tbluser.Authority  <= 15) ||
                        (tbluser.Authority >= 24 && tbluser.Authority <= 31))
                    {
                        Session["ApprovalFlag"] = "1";
                    }
                    else
                    {
                        Session["ApprovalFlag"] = "NA";
                    }
                    if(tbluser.Authority == 2 || tbluser.Authority == 3 ||
                     tbluser.Authority == 6 || tbluser.Authority == 7 || tbluser.Authority ==10 ||
                     tbluser.Authority == 11 || tbluser.Authority == 14 || tbluser.Authority == 15 ||
                     tbluser.Authority == 18 || tbluser.Authority == 19 || tbluser.Authority == 22 ||
                     tbluser.Authority == 23 || tbluser.Authority == 26 || tbluser.Authority == 27 ||
                     tbluser.Authority == 30 || tbluser.Authority == 31)
                    {
                        Session["MediaViewLog"] = "1";
                    }
                    else
                    {
                        Session["MediaViewLog"] = "0";
                    }
                    
                    if(tbluser.Authority == 1 || tbluser.Authority == 3|| tbluser.Authority == 5 ||
                        tbluser.Authority == 7 || tbluser.Authority == 17 || tbluser.Authority == 19 ||
                        tbluser.Authority == 21 || tbluser.Authority == 23)
                    {
                        Session["FeedBackApproval"] = "D";
                    }
                    else if(tbluser.Authority == 8 || tbluser.Authority == 10 || tbluser.Authority == 12 ||
                        tbluser.Authority == 14 || tbluser.Authority == 24 || tbluser.Authority == 26 ||
                        tbluser.Authority == 28 || tbluser.Authority == 30)
                    {
                        Session["FeedBackApproval"] = "A";
                    }
                    else if (tbluser.Authority == 9 || tbluser.Authority == 11 || tbluser.Authority == 13 ||
                        tbluser.Authority == 15 || tbluser.Authority == 25 || tbluser.Authority == 27 ||
                        tbluser.Authority == 29 || tbluser.Authority == 31)
                    {
                        Session["FeedBackApproval"] = "B";
                    }

                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    CommonLogic.Log_info(LogInfo.MenuClick, "User Entered Log in");
                    if (String.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        RedirectToLocal(returnUrl);
                    }
                       
                   
                 
                   
                }
                

                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntLoginSignInErrMsg;
                
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

            return RedirectToAction("Index");
        }

    
        public tblUser GetUserDetails(string userName)
        {
            tblUser tbluser = new tblUser();
            using (SqlConnection con = new SqlConnection(CS))
            {
                query = "";
                query = "SELECT UserID,UserName,UserEmail,Role,Authority From tblUser Where UserName = @UserName and DELFG = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    tbluser.UserID = Convert.ToInt32(rdr["UserID"]);
                    tbluser.UserName = rdr["UserName"].ToString();
                    tbluser.Authority = Convert.ToInt32(rdr["Authority"].ToString());
                    tbluser.UserEmail = rdr["UserEmail"].ToString();
                }
            }

            return tbluser;
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            CommonLogic.SetCultureInfo();
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Employer");
            }
        }
        public ActionResult SignOut()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            CommonLogic.SetCultureInfo();
            try
            {
                if (Session["StoreName"] == null || Session["StoreName"].ToString().Trim() == "")
                {
                    Session["StoreUserName"] = "Unknown";
                }
                else
                {
                    Session["StoreUserName"] = Session["StoreName"];
                }
                CommonLogic.Log_info(LogInfo.MenuClick, "User Sign out application");
                
                Session["UserName"] = "";
                Session["LoginType"] = "";
                Session["UserID"] = "";
                Session["UserEmail"] = "";
                Session["Role"] = "";
                Session["ApprovalFlag"] = "";
                Session["MediaViewLog"] = "";
                Session["FeedBackApproval"] = "";
                //// Sign Out.
                //authenticationManager.SignOut();
                FormsAuthentication.SignOut();
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                foreach (var cookie in Request.Cookies.AllKeys)
                {
                    Request.Cookies.Remove(cookie);
                }
                foreach (var cookie in Response.Cookies.AllKeys)
                {
                    Response.Cookies.Remove(cookie);
                }

                // Clear authentication cookie
                HttpCookie rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                rFormsCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(rFormsCookie);

                // Clear session cookie 
                HttpCookie rSessionCookie = new HttpCookie("ASP.NET_SessionId", "");
                rSessionCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(rSessionCookie);
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
         
           
        }

        // [AuthorizeIPAddress]
        [SessionExpire]
        [Authorize]
        public ActionResult ChangePassword()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            CommonLogic.SetCultureInfo();
            try
            {
 
                tblUser tbl_User = db.tblUsers.Find(Convert.ToInt32(Session["UserID"]));
                //tbl_User.OldPassword = tbl_User.Password;
                if (tbl_User == null)
                {
                    return HttpNotFound();
                }
          
                return View(tbl_User);
            }

            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }

        [SessionExpire]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "UserID,UserName,UserEmail,Password,CreateUserId,CRTCD,CRTDT,DELFG,Authority,Authority1,Authority2,Authority4,Authority8,Authority16,OldPassword,NewPassword,ConfirmNewPassword")] tblUser tbl_User)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            CommonLogic.SetCultureInfo();
            try
            {
                // PopulateRole();
                if (ModelState.IsValid)
                {
                    if (tbl_User.Password.ToString().Trim() != tbl_User.OldPassword.Trim())
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CntLoginChangePasswordErrMsg1;
                         return View(tbl_User);
                    }
                    if (tbl_User.NewPassword.ToString().Trim() != tbl_User.ConfirmNewPassword.Trim())
                    {
                        TempData["ErrMsg"] = MujiStore.Resources.Resource.CntLoginChangePasswordErrMsg2;
                         return View(tbl_User);
                    }

                    tbl_User.Role = "U";
                    db.Entry(tbl_User).State = EntityState.Modified;
                    
                    tbl_User.Password = tbl_User.NewPassword;
                    tbl_User.UPDDT = DateTime.Now;
                    tbl_User.UPDCD = Session["UserName"].ToString();
                    tbl_User.IPAddress = Session["IPAddress"].ToString();
 
                   
                    db.SaveChanges();
                    LogInfo.Comments = "User Updated - " + tbl_User.UserName.ToString();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntLoginChangePasswordErrSuccMsg;
                    
                    return RedirectToAction("ViewUploadDetails", "VideoFiles");
                }

                
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntLoginChangePasswordErrMsg3;
                return View(tbl_User);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = "Unable to Update Chnage Password";
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
    }
}