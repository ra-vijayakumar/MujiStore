using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MujiStore.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using MujiStore.BLL;
using PagedList;
using System.Threading;
using System.Globalization;

namespace MujiStore.Controllers
{
    //[SessionExpire]
    [AuthorizeIPAddress]
     //[CustomExceptionHandling]
    public class VideoFilesController : BaseController
    {
        string CS = ConfigurationManager.ConnectionStrings["cnstr"].ConnectionString;
      
        // GET: VideoFiles
        [NonAction]
        public ActionResult Index()
        {
            CommonLogic.SetCultureInfo();
            return View();
        }
        [NonAction]
        public ActionResult Create()
        {
            CommonLogic.SetCultureInfo();
            return View();
        }


        public string GetSessionUserorStoreName(string ipAddress)
        {
            string UserorStoreName;
            string query = "";
            if (Session["UserName"] == null || Session["UserName"].ToString() == "")
            {
                ipAddress = Session["IPAddress"].ToString();
                //string[] values = ipAddress.Split('.');
                //int lastPort = Convert.ToInt32(values[3].ToString());
                //string ipcuripAdd = values[0] + "." + values[1] + "." + values[2];
                UserorStoreName = ipAddress;

                if(Session["StoreName"].ToString() !=  "Unknown")
                {
                    UserorStoreName = Session["StoreName"].ToString();
                }
                //using (SqlConnection con = new SqlConnection(CS))
                //{
                //    query = "SELECT StoreID,SNIPAddress StoreIPAddress,StoreName,PARSENAME(SNIPAddress, 4)+'.'+";
                //    query += "PARSENAME(SNIPAddress, 3)+'.'+PARSENAME(SNIPAddress, 2) ipcuripAdd,";
                //    query += "PARSENAME(SNIPAddress, 1) lastPort FROM tblStore S ";
                //    query += "JOIN tblStoreSubnet SS on SS.Store = S.StoreID ";
                //    query += "JOIN tblSubnet SN on SN.SubnetID = SS.Subnet ";
                //    query += "WHERE S.DELFG=0 AND SS.DELFG = 0  AND SN.DELFG = 0";

                //    SqlCommand cmd = new SqlCommand(query, con);
                //    cmd.CommandType = CommandType.Text;
                //    con.Open();
                //    SqlDataReader rdr = cmd.ExecuteReader();
                //    while (rdr.Read())
                //    {
                //        if (rdr["lastPort"].ToString() != "::1")
                //        {
                //            if (Convert.ToInt16(rdr["lastPort"].ToString()) == 0)
                //            {
                //                if (ipcuripAdd == rdr["ipcuripAdd"].ToString())
                //                {

                //                    if (lastPort >= Convert.ToInt16(rdr["lastPort"].ToString()) && lastPort <= 255)
                //                    {
                //                        UserorStoreName = rdr["StoreName"].ToString();
                //                        break;
                //                    }
                //                }
                //            }
                //        }
                //        else
                //        {
                //            if (ipAddress == rdr["StoreIPAddress"].ToString())
                //            {
                //                UserorStoreName = rdr["StoreName"].ToString();
                //                break;
                //            }
                //        }
                //    }
                //}
                //model.userid = 0;
            }
            else
            {
                UserorStoreName = Session["UserName"].ToString();
                //model.userid = 100;
            }

            return UserorStoreName;
        }
        [HttpPost]
        public ActionResult SaveThumbnailImage(tblFeedback model)
        {
            string query = "";
            IPAddressDtl ipdtl = new IPAddressDtl();
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            CommonLogic.SetCultureInfo();
            try
            {
                var fileName = "";
                var path = Server.MapPath("~/FeedBack");

                if (model.PostedFile != null)
                {
  
                    fileName = CommonLogic.ModifiedFileName(model.PostedFile.FileName);
                    //get full file path
                    var fileNameWitPath = Path.Combine(path, fileName);
                    model.PostedFile.SaveAs(fileNameWitPath);
                }
             
                model.WriterName = GetSessionUserorStoreName(Session["IPAddress"].ToString());

                if (model.WriterName.Trim().Length == 0)
                {
                    model.WriterName = Session["IPAddress"].ToString();
                }
                LogInfo.Comments = "Feedback Created - " + model.Comments;
                using (SqlConnection con = new SqlConnection(CS))
                    {
                        query = "Insert into tblFeedback(MovieID,WriterName,Comments,FileName,IPAddress,CRTCD,CRTDT,DELFG) Values ";
                        query += "(@MovieID,@WriterName,@Comments,@FileName,@IPAddress,@CRTCD,@CRTDT,@DELFG);";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@MovieID", model.MovieID);
                        cmd.Parameters.AddWithValue("@WriterName", model.WriterName);
                        cmd.Parameters.AddWithValue("@Comments", model.Comments);
                        cmd.Parameters.AddWithValue("@FileName", fileName);
                        cmd.Parameters.AddWithValue("@IPAddress", Session["IPAddress"].ToString());
                        cmd.Parameters.AddWithValue("@CRTCD", model.WriterName);
                        cmd.Parameters.AddWithValue("@CRTDT", DateTime.Now);
                        cmd.Parameters.AddWithValue("@DELFG", true);
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        int result = cmd.ExecuteNonQuery();
                        CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                }
          
                return RedirectToAction("ViewUploadDetailsByID", "VideoFiles", new { id = model.MovieID, folderID = model.FolderID });
              }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        public ActionResult ViewUploadDetails()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            CommonLogic.SetCultureInfo();
            try
            {
                int FolderID = -1;

                var headFolderStructue = getFolderStructure(FolderID);
                ViewData["headFolderStructue"] = headFolderStructue;

                var childFolderDtl = getChildFolder(FolderID);
                ViewData["ChildFolderDtl"] = childFolderDtl;

                var mediaFileDtl = getMedialFileDetails(FolderID);
                ViewData["VideoFileDetails"] = mediaFileDtl;
                return View("ViewUploadDetailsNew");
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }

        public List<tblFeedback> getFeedBackDetails(int MediaID)
        {
            List<tblFeedback> fblist = new List<tblFeedback>();
            string query = "";
            using (SqlConnection con = new SqlConnection(CS))
            {
                query = "select FeedbackID,WriterName,Comments,FileName,WriterDatetime from tblFeedback where MovieID = @MovieID and DELFG=0 Order By FeedbackID Desc";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MovieID", MediaID);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    tblFeedback fback = new tblFeedback();
                    fback.FeedbackID = Convert.ToInt32(rdr["FeedbackID"]);
                    fback.WriterName = rdr["WriterName"].ToString();
                    fback.Comments = rdr["Comments"].ToString();
                    fback.FileName = rdr["FileName"].ToString();
                    fback.WriterDatetime = Convert.ToDateTime(rdr["WriterDatetime"]);
                    fblist.Add(fback);
                }
            }

            return fblist;
        }

        public tblDeployStatu GetDeployStatus(int mediaID)
        {
            List<tblDeployStatu> dpsdtllist = new List<tblDeployStatu>();
            tblDeployStatu dpstdtl = new tblDeployStatu();
            bool depserIsExists = false;
            if (Session["SubnetID"].ToString() != "-1")
            {
                dpsdtllist = BLL.CommonLogic.GetDSWithSS(mediaID, Convert.ToInt32(Session["SubnetID"]));
                if (dpsdtllist == null || dpsdtllist.Count == 0)
                {
                    dpsdtllist = BLL.CommonLogic.GetDSWithSNSS(mediaID, Convert.ToInt32(Session["SubnetID"]));
                }
                if (dpsdtllist != null && dpsdtllist.Count > 0)
                {
                    depserIsExists = true;
                    dpstdtl.DSServer = dpsdtllist[0].DSServer;
                    dpstdtl.IPAddress = dpsdtllist[0].IPAddress;
                    dpstdtl.FormatName = dpsdtllist[0].FormatName;
                    dpstdtl.Recommend = dpsdtllist[0].Recommend;
                    //FileDtl.ServerIP 

                    //dpsdtl = GetDSWithSNSS(ID, Convert.ToInt32(Session["SubnetID"]));
                }
            }

            return (tblDeployStatu)dpstdtl;
            //if (depserIsExists == true)
            //{
            //    ViewData["ServerDtl"] = (tblDeployStatu)dpstdtl;
            //}
            //else
            //{
            //    ViewData["ServerDtl"] = null;
            //}
        }
        public tblMedia getMediaDetails(int MediaID)
        {
            tblMedia mediaDtl = new tblMedia();
            string query = "";
            using (SqlConnection con = new SqlConnection(CS))
            {
                query = "Select MediaID,Title,Description,Video,Thumbnail,FolderID,ViewCount from [tblmedia] where MediaID =@MediaID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MediaID", MediaID);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    
                    mediaDtl.MediaID = Convert.ToInt32(rdr["MediaID"]);
                    mediaDtl.Title = rdr["Title"].ToString();
                    mediaDtl.Description = rdr["Description"].ToString();
                    mediaDtl.Video = rdr["Video"].ToString();
                    mediaDtl.Thumbnail = rdr["Thumbnail"].ToString();
                    mediaDtl.FolderID = Convert.ToInt32(rdr["FolderID"]);
                    mediaDtl.ViewCount = Convert.ToInt32(rdr["ViewCount"]);
 
                }
            }

            return mediaDtl;
        }

      


        public List<tblMedia> getMedialFileDetails(int folderID)
        {
            List<tblMedia> videolist = new List<tblMedia>();
            string query = "";
            using (SqlConnection con = new SqlConnection(CS))
            {
                query = "Select MediaID,Title,Description,Video,Thumbnail,FolderID,Duration,ViewCount from [tblmedia] where FolderID =@FolderID and DELFG = 0 and ConvertStatus >= 3 and PhysicalDELFG = 0 and ApprovalStatus >= 3 order by MediaID desc";//VJ 20200603 ConvertStatus 3 Converted  Changed
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FolderID", folderID);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    tblMedia video = new tblMedia();
                    video.MediaID = Convert.ToInt32(rdr["MediaID"]);
                    video.Title = rdr["Title"].ToString();
                    video.Description = rdr["Description"].ToString();
                    video.Video = rdr["Video"].ToString();
                    video.Thumbnail = rdr["Thumbnail"] .ToString();
                    video.FolderID = Convert.ToInt32(rdr["FolderID"]);
                    video.Duration = Convert.ToInt32(rdr["Duration"]);
                    video.ViewCount = Convert.ToInt32(rdr["ViewCount"]);
    
                    videolist.Add(video);
                }
            }

            return videolist;
        }
        public string GetFileDuration(int duration)
        {
            return CommonLogic.GetFileDuration(duration);
        }
        public List<tblFolder> getFolderStructure(int folderID)
        {
            List<tblFolder> folderlist = new List<tblFolder>();
            string query = "";
            tblFolder defolder = new tblFolder();
            defolder.FolderID = -1;
            defolder.ParentID = 0;
            defolder.Name = "目次";
            folderlist.Add(defolder);
            using (SqlConnection con = new SqlConnection(CS))
            {
                query = "WITH tblParent AS ( SELECT * FROM tblFolder WHERE DELFG = 0 AND FolderID = @id UNION ALL SELECT tblFolder.*";
                query += " FROM tblFolder  JOIN tblParent  ON tblFolder.FolderID = tblParent.ParentId Where tblFolder.DELFG = 0) ";
                query += " SELECT * FROM  tblParent order by ParentID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", folderID);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    tblFolder folder = new tblFolder();
                    folder.FolderID = Convert.ToInt32(rdr["FolderID"]);
                    folder.ParentID = Convert.ToInt32(rdr["ParentID"]);
                    folder.Name = rdr["Name"].ToString();
                    folderlist.Add(folder);
                }
            }

            return folderlist;
        }

        public List<tblFolder> getChildFolder(int parentId)
        {
            
            List<tblFolder> folderlist = new List<tblFolder>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("Select FolderID,ParentID,Name from [tblFolder] WHERE ParentID=@ParentID and DELFG = 0 Order by FolderID Desc", con);
                cmd.Parameters.AddWithValue("@ParentID", parentId);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    tblFolder folder = new tblFolder();
                    folder.FolderID = Convert.ToInt32(rdr["FolderID"]);
                    folder.ParentID = Convert.ToInt32(rdr["ParentID"]);
                    folder.Name = rdr["Name"].ToString();
                    folderlist.Add(folder);
                }
            }

            return folderlist;
        }
        [MUJICustomAuthorize(Roles = "16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31")]
        public ActionResult MediaViewLog(int MediaID,int? page)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            CommonLogic.SetCultureInfo();
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
                tblMedia Mediadtl = new tblMedia();
                
                 Mediadtl = getMediaDetails(MediaID);
                List<tblMediaViewLog> mvList = new List<tblMediaViewLog>();
                VideoFeedBack VFeedback = new VideoFeedBack();
                VFeedback.FolderID = Mediadtl.FolderID;
                VFeedback.VideoId = Mediadtl.MediaID;
                VFeedback.VideoTitle = Mediadtl.Title;
                ViewData["VFeedback"] = VFeedback;

                string query = "";
                using (SqlConnection con = new SqlConnection(CS))
                {
                    query = "select top 100 CRTDT,StoreIPAddress,StoreName,UserName,ClientIP from tblMediaViewLog";
                    query += " where MediaID = @MediaID and delfg = 0 order by MediaViewLogID desc";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MediaID", MediaID);
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        tblMediaViewLog medlog = new tblMediaViewLog();
                        medlog.CRTDT = Convert.ToDateTime(rdr["CRTDT"]);
                        medlog.StoreIPAddress = rdr["StoreIPAddress"].ToString();
                        medlog.StoreName = rdr["StoreName"].ToString();
                        medlog.UserName = rdr["UserName"].ToString();
                        medlog.ClientIP = rdr["ClientIP"].ToString();

                        mvList.Add(medlog);
                    }
                }

                  return View(mvList.ToPagedList(pageNumber,pageSize));
 
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        public ActionResult ShowFolderDetails(int folderID)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            CommonLogic.SetCultureInfo();
            try
            {
                var headFolderStructue = getFolderStructure(folderID);
                ViewData["headFolderStructue"] = headFolderStructue;

                var childFolderDtl = getChildFolder(folderID);
                ViewData["ChildFolderDtl"] = childFolderDtl;

                var mediaFileDtl = getMedialFileDetails(folderID);
                ViewData["VideoFileDetails"] = mediaFileDtl;
   
                return View("ViewUploadDetailsNew");
            }
            catch(Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
    
        }
     
        public ActionResult ViewMeidaLogDetailsByID(int ID, int folderID)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            CommonLogic.SetCultureInfo();
            try
            {

                VideoFeedBack VFeedback = new VideoFeedBack();
                VFeedback.FolderID = folderID;
                VFeedback.VideoId = ID;
                ViewData["VFeedback"] = VFeedback;
                var headFolderStructue = getFolderStructure(folderID);
                ViewData["headFolderStructue"] = headFolderStructue;

                var childFolderDtl = getChildFolder(folderID);
                ViewData["ChildFolderDtl"] = childFolderDtl;

                var mediaFileDtl = getMedialFileDetails(folderID);
                ViewData["VideoFileDetails"] = mediaFileDtl;

                var feedBackDtl = getFeedBackDetails(VFeedback.VideoId);
                ViewData["feedBackDtl"] = feedBackDtl;
                ViewData["ServerDtl"] = GetDeployStatus(ID);

                var FileDtl = mediaFileDtl.Where(a => a.MediaID == ID).SingleOrDefault();
                if (FileDtl == null)
                {
                    return View("ViewUploadDetailsNew");
                }
               
                LogInfo.Comments = "Media Log back to Video File - " + FileDtl.Title.ToString();
                CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
               
                ViewData["FileDetails"] = (tblMedia)FileDtl;
   
                return View("ViewUploadDetailsNew");
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }
        public ActionResult ViewUploadDetailsByID(int ID,int folderID)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            CommonLogic.SetCultureInfo();
            try
            {
                string query = "";
                string username;


                VideoFeedBack VFeedback = new VideoFeedBack();
                VFeedback.FolderID = folderID;
                VFeedback.VideoId = ID;
                ViewData["VFeedback"] = VFeedback;
                var headFolderStructue = getFolderStructure(folderID);
                ViewData["headFolderStructue"] = headFolderStructue;

                var childFolderDtl = getChildFolder(folderID);
                ViewData["ChildFolderDtl"] = childFolderDtl;

                var mediaFileDtl = getMedialFileDetails(folderID);
                ViewData["VideoFileDetails"] = mediaFileDtl;

                var feedBackDtl = getFeedBackDetails(VFeedback.VideoId);
                ViewData["feedBackDtl"] = feedBackDtl;
                var FileDtl = mediaFileDtl.Where(a => a.MediaID == ID).SingleOrDefault();
                if (FileDtl == null)
                {
                    return View("ViewUploadDetailsNew");
                }

               
                    username = GetSessionUserorStoreName(Session["IPAddress"].ToString());

                    if (username.Trim().Length == 0)
                    {
                        username = Session["IPAddress"].ToString();
                    }
                    using (SqlConnection con = new SqlConnection(CS))
                    {

                        query = "Insert into [tblMediaViewLog](MediaID,UserName,ClientIP,UserAgent,StoreID,StoreIPAddress,StoreName,CRTCD) Values ";
                        query += "(@MediaID,@UserName,@ClientIP,@UserAgent,@StoreID,@StoreIPAddress,@StoreName,@CRTCD);";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@MediaID", ID);
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@ClientIP", Session["IPAddress"].ToString());
                        cmd.Parameters.AddWithValue("@UserAgent", Request.ServerVariables["HTTP_USER_AGENT"]);
                        cmd.Parameters.AddWithValue("@StoreID", Session["StoreID"].ToString());
                        cmd.Parameters.AddWithValue("@StoreIPAddress", Session["StoreIPAddress"].ToString());
                        cmd.Parameters.AddWithValue("@StoreName", Session["StoreName"].ToString());
                        cmd.Parameters.AddWithValue("@CRTCD", username);
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        int result = cmd.ExecuteNonQuery();
                    }
                    using (SqlConnection con = new SqlConnection(CS))
                    {
                        query = "";
                        query = "UPDATE tblMedia SET ViewCount = ViewCount + 1 WHERE MediaID =@MediaID";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@MediaID", ID);
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        int result = cmd.ExecuteNonQuery();
                    }
                    LogInfo.Comments = "Media File Watched - " + FileDtl.Title.ToString();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    FileDtl.ViewCount += 1;

                //List<tblDeployStatu> dpsdtllist = new List<tblDeployStatu>();
                //tblDeployStatu dpstdtl = new tblDeployStatu();
                //bool depserIsExists = false;
                //if (Session["SubnetID"].ToString() != "-1")
                //{
                //    dpsdtllist = BLL.CommonLogic.GetDSWithSS(ID, Convert.ToInt32(Session["SubnetID"]));
                //    if(dpsdtllist == null || dpsdtllist.Count == 0)
                //    {
                //        dpsdtllist = BLL.CommonLogic.GetDSWithSNSS(ID, Convert.ToInt32(Session["SubnetID"]));
                //    }
                //    if (dpsdtllist != null && dpsdtllist.Count > 0)
                //    {
                //        depserIsExists = true;
                //        dpstdtl.DSServer = dpsdtllist[0].DSServer;
                //        dpstdtl.IPAddress = dpsdtllist[0].IPAddress;
                //        dpstdtl.FormatName = dpsdtllist[0].FormatName;
                //        dpstdtl.Recommend = dpsdtllist[0].Recommend;
                //        //FileDtl.ServerIP 

                //        //dpsdtl = GetDSWithSNSS(ID, Convert.ToInt32(Session["SubnetID"]));
                //    }
                //}
                ViewData["ServerDtl"] = GetDeployStatus(ID);
                //if (depserIsExists == true)
                //{
                //    ViewData["ServerDtl"] = (tblDeployStatu)dpstdtl;
                //}
                //else
                //{
                //    ViewData["ServerDtl"] = null;
                //}
                
                //         public string DSServer { get; set; }
                //public string ServerIP { get; set; }
                //public string FormatName { get; set; }
                //public bool Recommend { get; set; }

                ViewData["FileDetails"] = (tblMedia)FileDtl;


                return View("ViewUploadDetailsNew");
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
            
        }
     
    }
}