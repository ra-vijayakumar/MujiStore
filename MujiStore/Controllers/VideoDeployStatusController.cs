using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MujiStore.Models;
using System.Configuration;
using System.Data.SqlClient;
using MujiStore.BLL;
using System.Diagnostics;
using PagedList;
using System.Text;
using System.Threading;

namespace MujiStore.Controllers
{
    [SessionExpire]
    [Authorize]
    // [Authorize(Roles = "A,U")]
    [MUJICustomAuthorize(Roles = "8,9,10,11,12,13,14,15,24,25,27,26,28,29,30,31")]
    public class VideoDeployStatusController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();
        string CS = ConfigurationManager.ConnectionStrings["cnstr"].ConnectionString;

        [SessionExpire]
        [Authorize]
        // [Authorize(Roles = "A,U")]
        [MUJICustomAuthorize(Roles = "8,9,10,11,12,13,14,15,24,25,27,26,28,29,30,31")]

        // GET: VideoDeployStatus
        public ActionResult Index(string sortOrder, int? page, string SearTitle, string SearchTitle, string SearchFromCRTDT, string CdStDate, string SearchToCRTDT, string CdEdDate, int? ConvertStatus, int? CvtStatus, int? FolderID, int? FolID)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            int pageSize;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDSortParm = sortOrder == "ID" ? "ID_desc" : "ID";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.ConvertStatusSortParm = sortOrder == "ConvertStatus" ? "ConvertStatus_desc" : "ConvertStatus";
            ViewBag.FolderSortParm = sortOrder == "Folder" ? "Folder_desc" : "Folder";

            //List<SelectListItem> lstDelFlag = new List<SelectListItem>();

            //lstDelFlag.Add(new SelectListItem { Text = "All", Value = "2" });
            //lstDelFlag.Add(new SelectListItem { Text = "No", Value = "0" });
            //lstDelFlag.Add(new SelectListItem { Text = "Yes", Value = "1" });
            //ViewBag.DelFlag = new SelectList(lstDelFlag, "Value", "Text", DFlag);

            List<SelectListItem> lstConvertStatus = new List<SelectListItem>();

            lstConvertStatus.Add(new SelectListItem { Text = "All", Value = "4" });
            lstConvertStatus.Add(new SelectListItem { Text = "Approved", Value = "3" });//VJ 20200603 ConvertStatus Changed
            lstConvertStatus.Add(new SelectListItem { Text = "Un Approved", Value = "0" });
            ViewBag.ConvertStatus = new SelectList(lstConvertStatus, "Value", "Text", CvtStatus);

            List<SelectListItem> lstFolderName = new List<SelectListItem>();
            List<SelectListItem> lstFolderNameBulk = new List<SelectListItem>();
            lstFolderName = BLL.CommonLogic.FillFolderList();
            lstFolderNameBulk = BLL.CommonLogic.FillFolderList();
            ViewBag.FolderList = new SelectList(lstFolderNameBulk, "Value", "Text");
            lstFolderName.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.FolderID = new SelectList(lstFolderName, "Value", "Text", FolID);

            if (SearchTitle != null || SearchFromCRTDT != null || SearchToCRTDT != null || ConvertStatus != null)
            {
                page = 1;
            }
            else
            {
                SearchTitle = SearTitle;
                SearchFromCRTDT = CdStDate;
                SearchToCRTDT = CdEdDate;
                ConvertStatus = CvtStatus;
                // DelFlag = DFlag;
                FolderID = FolID;
            }
            ViewBag.SearTitle = SearchTitle;
            ViewBag.CdStDate = SearchFromCRTDT;
            ViewBag.CdEdDate = SearchToCRTDT;
            ViewBag.cvtStatus = ConvertStatus;
            // ViewBag.DFlag = DelFlag;
            ViewBag.FolID = FolderID;
            if (ViewBag.CdStDate == null || ViewBag.CdStDate == "")
            {
                CdStDate = DateTime.Now.AddYears(-200).ToString("yyyy/MM/dd");
            }
            else
            {
                CdStDate = ViewBag.CdStDate;
            }
            if (ViewBag.CdEdDate == null || ViewBag.CdEdDate == "")
            {
                CdEdDate = DateTime.Now.AddYears(100).ToString("yyyy/MM/dd");
            }
            else
            {
                CdEdDate = ViewBag.CdEdDate;
            }
            //New
            try
            {
               

                FillApprovalStatus();
              
              

                //var videoList="";
                List<tblMedia> videoList = new List<tblMedia>();
                //var videoList;
                string uName = Session["UserName"].ToString();
                ViewData["FolderDtl"] = BLL.CommonLogic.FillFolderList();
                string query = "select MediaID,Title,Description,Video,ConvertStatus,tblMedia.FolderID,Name,ApprovalStatus ";
                query = query + "from [dbo].[tblMedia] left join tblFolder on tblMedia.FolderID = tblFolder.FolderID where ";
                query = query + " PhysicalDELFG = 0 ";
                query = query + " and FORMAT(tblMedia.CRTDT, 'yyyy-MM-dd') between '" + CdStDate + "' and '" + CdEdDate + "'";
                //query = query + " lastName like '%" + Name + "%' and EnrollmentDate ";
             

                if (ViewBag.SearTitle != null && ViewBag.SearTitle != "" && ViewBag.SearTitle != string.Empty)
                {
                    query = query + " and title like '%" + ViewBag.SearTitle + "%'";
                }

                if (ViewBag.cvtStatus != null && ViewBag.cvtStatus != 4)
                {
                    query = query + " and ApprovalStatus = " + ViewBag.cvtStatus;
                }
                if (ViewBag.FolID != null && ViewBag.FolID != 0)
                {
                    query = query + " and tblMedia.FolderID = " + Convert.ToInt32(ViewBag.FolID);
                }
                query = query + " Order by MediaID Desc";

                using (SqlConnection con = new SqlConnection(CS))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                videoList.Add(new tblMedia
                                {
                                    // MediaID,Title,Description,Video,ConvertStatus,FolderID
                                    MediaID = Convert.ToInt32(sdr["MediaID"]),
                                    Title = Convert.ToString(sdr["Title"]),
                                    Description = Convert.ToString(sdr["Description"]),
                                    Video = Convert.ToString(sdr["Video"]),
                                    ConvertStatus = Convert.ToInt32(sdr["ConvertStatus"]),
                                    FolderID = Convert.ToInt32(sdr["FolderID"]),
                                    FolderName = Convert.ToString(sdr["Name"]),
                                    ApprovalStatus = Convert.ToInt32(sdr["ApprovalStatus"])
                                });
                            }
                        }
                        con.Close();
                    }
                }


                switch (sortOrder)
                {
                    //case "ID_desc":
                    //    videoList = videoList.OrderByDescending(o => o.MediaID).ToList();
                    //    break;
                    case "ID":
                        videoList = videoList.OrderBy(o => o.MediaID).ToList();
                        break;
                    case "Title":
                        videoList = videoList.OrderBy(o => o.Title).ToList();
                        break;
                    case "Title_desc":
                        videoList = videoList.OrderByDescending(o => o.Title).ToList();
                        break;
                    case "ConvertStatus_desc":
                        videoList = videoList.OrderByDescending(o => o.ApprovalStatus).ToList();
                        break;
                    case "ConvertStatus":
                        videoList = videoList.OrderBy(o => o.ApprovalStatus).ToList();
                        break;
                    case "Folder_desc":
                        videoList = videoList.OrderByDescending(o => o.FolderName).ToList();
                        break;
                    case "Folder":
                        videoList = videoList.OrderBy(o => o.FolderName).ToList();
                        break;
                    default:  // Name ascending 
                        videoList = videoList.OrderByDescending(o => o.MediaID).ToList();
                        break;
                }

                if (System.Configuration.ConfigurationManager.AppSettings["PageSize"] == null)
                {
                    pageSize = 10;
                }
                else
                {
                    pageSize = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
                }

                int pageNumber = (page ?? 1);

                ViewData["videoList"] = videoList.ToPagedList(pageNumber, pageSize);


                return View();
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }


        }

        public ActionResult Edit(int? id)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;

            try
            {
                string uName = Session["UserName"].ToString();
                FillApprovalStatus();
                List<SelectListItem> lstFolderName = new List<SelectListItem>();
                lstFolderName = BLL.CommonLogic.FillFolderList();
                lstFolderName.Insert(0, (new SelectListItem { Text = "Select", Value = "0" }));
                ViewData["FolderDtl"] = lstFolderName;

                List<SelectListItem> lstRobocopyExitcodeList = new List<SelectListItem>();
                ViewData["roboResult"] = FillRobocopyExitcodeList();
                // ViewBag.Result = new SelectList(db.tblRobocopyExitcodes.Where(x => x.DELFG == false), "RobocopyExitcodeID", "Content", tblStreamServerSubnet.Subnet);
                // ViewBag.Result = new SelectList(db.tblRobocopyExitcodes.Where(x => x.DELFG == false), "RobocopyExitcodeID", "Content");
                // ViewData["FolderDtl"] = FillList();

                // ViewData["MediaServer"]= GetMediaServerIPAddress(id);

                tblMedia tblVideoDemo = new tblMedia();
                tblFolder tblfol = new tblFolder();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                //int fid = 0;
                tblVideoDemo = db.tblMedias.Where(x => x.PhysicalDELFG == false && x.MediaID == id).SingleOrDefault();
               // fid = tblVideoDemo.FolderID;
                tblfol = db.tblFolders.Where(x => x.FolderID == tblVideoDemo.FolderID).SingleOrDefault();
                tblVideoDemo.FolderName = tblfol.Name;
                //tblMedia tblVideoDemo = db.tblMedias.Find(id);
                if (tblVideoDemo == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    tblVideoDemo.ThumbnailFileName = tblVideoDemo.Thumbnail;
                }

                if (Session["ApprovalFlag"].ToString() == "1")
                {
                    tblVideoDemo.deployStatus = GetDeployStatus(id);
                }



                return View(tblVideoDemo);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }
        public void FillApprovalStatus()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Un Approved", Value = "0" });
            list.Add(new SelectListItem { Text = "Approved", Value = "3" });
            ViewData["ApprovalDtl"] = list;
        }

        public IEnumerable<tblDeployStatu> GetDeployStatus(int? id)
        {
            //IEnumerable<tblDeployStatu> depstatus = new List<tblDeployStatu>();
            var depstatus = new List<tblDeployStatu>();
            string querySDtnt = "";
            tblMedia med = new tblMedia();
            med = db.tblMedias.Where(x => x.MediaID == id).SingleOrDefault();

            string fileName = med.Video;
            FileInfo fi = new FileInfo(fileName);
            string extn = fi.Extension.TrimStart('.');

            tblFormat formt = new tblFormat();
            formt = db.tblFormats.Where(x => x.Name.ToUpper() == extn.ToUpper()).SingleOrDefault();

            using (SqlConnection con = new SqlConnection(CS))
            {
                querySDtnt = " Select isnull(DeployStatusID,0) DeployStatusID,SSServer DSServer, ";
                querySDtnt += "isnull(DeployLogID, 0) DeployLogID, ";
                querySDtnt += " DS.MediaID,DS.FormatID,isnull(IsExists,0) IsExists, isnull(Result,0) Result, ";
                querySDtnt += "isnull(M.Duration,0) Duration,isnull(M.FileSize,0) FileSize ";
                querySDtnt += "from  ( ";
                querySDtnt += " 	select SSServer SSServer from tblStreamServer ";
                querySDtnt += " 	union ";
                querySDtnt += "    select SSSServer SSServer from tblStreamServerSubnet ";
                querySDtnt += ") SServer ";
                querySDtnt += " left join tblDeployStatus DS on DS.DSServer = SSServer  And  DS.MediaID =@MediaID";
                querySDtnt += " left join tblDeployLog DL on DL.Server = SSServer  And  DL.MediaID =@MediaID";
                querySDtnt += " left Join tblMedia M on M.MediaID =@MediaID";
                querySDtnt += " Order by  SSServer ";

                SqlCommand cmds = new SqlCommand(querySDtnt, con);
                cmds.Parameters.AddWithValue("@MediaID", id);
                cmds.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdrs = cmds.ExecuteReader();
                while (rdrs.Read())
                {

                    depstatus.Add(new tblDeployStatu
                    {
                        DeployStatusID = Convert.ToInt32(rdrs["DeployStatusID"]),
                        DeployLogID = Convert.ToInt32(rdrs["DeployLogID"]),
                        DSServer = rdrs["DSServer"].ToString(),
                        MediaID = id,
                        FormatID = formt.FormatID,
                        Duration = Convert.ToInt32(rdrs["Duration"]),
                        FileSize = Convert.ToInt32(rdrs["FileSize"]),
                        Result = Convert.ToInt16(rdrs["Result"]),
                        DELFG = Convert.ToBoolean(rdrs["IsExists"])
                    });


                }
            }
            return depstatus;
        }

        public List<SelectListItem> FillRobocopyExitcodeList()
        {
            string query = "";
            var list = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                //SqlCommand cmd = new SqlCommand("Select VideoId,Title,Description,Video,Thumbnail,FoderID from [tblVideoDemo]", con);
                query = "Select RobocopyExitcodeID,Content from tblRobocopyExitcode where Delfg = 0 Order by Content ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                list.Add(new SelectListItem { Text = "Select", Value = "0" });
                while (rdr.Read())
                {
                    list.Add(new SelectListItem { Text = rdr["Content"].ToString(), Value = Convert.ToInt32(rdr["RobocopyExitcodeID"]).ToString() });

                }
            }

            // list.Add(new SelectListItem { Text = "Instructor", Value = "Instructor" });
            //list.Add(new SelectListItem { Text = "Student", Value = "Student" });


            return list;
        }

        [HttpPost]
        public ActionResult SaveVideoDetails(tblMedia _tblVideoDemo)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            int result;
            try
            {
       

                List<SelectListItem> lstRobocopyExitcodeList = new List<SelectListItem>();
                ViewData["roboResult"] = FillRobocopyExitcodeList();

  

               
                    if (_tblVideoDemo.deployStatus != null && _tblVideoDemo.PhysicalDELFG == false && _tblVideoDemo.deployStatus.Count() > 0)
                    {
                        int i = 0;
                        foreach (var bupload in _tblVideoDemo.deployStatus)
                        {
                            if (bupload.DELFG == true && (bupload.Result == null || bupload.Result == 0))
                            {
                                ModelState.AddModelError("deployStatus[" + i + "].Result", MujiStore.Resources.Resource.SelectRobocopyExitcodeContent);
                                return View("Edit", _tblVideoDemo);
                            }
                            i += 1;
                        }
                        result = UpdateDeployStatusAll(_tblVideoDemo.deployStatus);
                    }


                LogInfo.Comments = "Video Deploy stauus Details Updated - " + _tblVideoDemo.Title.ToString();
                CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                //db.SaveChanges();
                TempData["SuccMsg"] = MujiStore.Resources.Resource.CntVideoDemoSaveSaveVideoDetailsSuccMsg;
                //do somthing with model
                return RedirectToAction("Index", "VideoDeployStatus");
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntVideoDemoSaveSaveVideoDetailsErrMsg;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }

        public int UpdateDeployStatusAll(IEnumerable<tblDeployStatu> _tblDeployStatus)
        {
            int result = 0;

            //tblMedia tblmed = new tblMedia();

            // var tblmed = db.tblMedias.Where(x => x.MediaID == _tblDeployStatus.FirstOrDefault().MediaID);

            //tblMedia tblVideoDemo = db.tblMedias.Find(_tblDeployStatus.FirstOrDefault().MediaID);

            foreach (var bupload in _tblDeployStatus)
            {
                if (bupload.DeployStatusID > 0)
                {
                    //db.tblDeployStatus()
                    bupload.IsExists = bupload.DELFG;
                    bupload.DELFG = bupload.DELFG == true ? false : true;
                    bupload.DateTime = DateTime.Now;
                    bupload.UPDDT = DateTime.Now;
                    bupload.UPDCD = Session["UserName"].ToString();
                    bupload.UserIPAddress = Session["IPAddress"].ToString();
                    bupload.Duration = bupload.Duration;
                    bupload.FileSize = bupload.FileSize;
                    UpdateDeployStatus(bupload);
                    //db.Entry(bupload).State = EntityState.Modified;
                    //db.SaveChanges();
                }
                else if (bupload.DeployStatusID == 0 && bupload.DELFG == true)
                {

                    bupload.IsExists = bupload.DELFG;
                    bupload.DateTime = DateTime.Now;
                    bupload.DELFG = bupload.DELFG == true ? false : true;
                    bupload.CRTDT = DateTime.Now;
                    bupload.CRTCD = Session["UserName"].ToString();
                    bupload.UserIPAddress = Session["IPAddress"].ToString();
                    bupload.Duration = bupload.Duration;
                    bupload.FileSize = bupload.FileSize;
                    InsertDeployStatus(bupload);

                }
            }
            return result;
        }

        public int UpdateDeployStatus(tblDeployStatu _tblDeployStatus)
        {
            int result = 0;
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = "";


                con.Open();
                SqlCommand cmd;
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = con.BeginTransaction("SampleTransaction");
                //cmd.Connection = con;
                try
                {
                    query = "Update tblDeployStatus set FormatID=@FormatID,IsExists=@IsExists,DateTime=@DateTime,";
                    query += "DELFG=@DELFG,UPDDT=@UPDDT,UPDCD=@UPDCD,UserIPAddress=@UserIPAddress";
                    query += " Where DeployStatusID=@DeployStatusID";

                    cmd = new SqlCommand(query, con);
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@FormatID", _tblDeployStatus.FormatID);
                    cmd.Parameters.AddWithValue("@IsExists", _tblDeployStatus.IsExists);
                    cmd.Parameters.AddWithValue("@DateTime", _tblDeployStatus.DateTime);
                    cmd.Parameters.AddWithValue("@DELFG", _tblDeployStatus.DELFG);
                    cmd.Parameters.AddWithValue("@UPDDT", _tblDeployStatus.UPDDT);
                    cmd.Parameters.AddWithValue("@UPDCD", _tblDeployStatus.UPDCD);
                    cmd.Parameters.AddWithValue("@UserIPAddress", _tblDeployStatus.UserIPAddress);
                    cmd.Parameters.AddWithValue("@DeployStatusID", _tblDeployStatus.DeployStatusID);
                    cmd.CommandType = CommandType.Text;

                    result = cmd.ExecuteNonQuery();

                    query = "";
                    query = "Update tblDeployLog set DELFG=@DELFG,UPDDT=@UPDDT,UPDCD=@UPDCD";
                    if (_tblDeployStatus.DELFG == false)
                    {
                        query += ",Result = @Result";
                    }
                    query += ",IPAddress=@IPAddress";
                    query += " Where DeployLogID=@DeployLogID ";

                    cmd = new SqlCommand(query, con);
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@DeployLogID", _tblDeployStatus.DeployLogID);
                    if (_tblDeployStatus.DELFG == false)
                    {
                        cmd.Parameters.AddWithValue("@Result", _tblDeployStatus.Result);
                    }
                    cmd.Parameters.AddWithValue("@DELFG", _tblDeployStatus.DELFG);
                    cmd.Parameters.AddWithValue("@UPDDT", _tblDeployStatus.UPDDT);
                    cmd.Parameters.AddWithValue("@UPDCD", _tblDeployStatus.UPDCD);
                    cmd.Parameters.AddWithValue("@IPAddress", _tblDeployStatus.UserIPAddress);
                    cmd.CommandType = CommandType.Text;

                    result = cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                    Log.Error(LogInfo.LogMsg, ex);
                }
            }

            return result;
        }

        public int InsertDeployStatus(tblDeployStatu _tblDeployStatus)
        {
            int result = 0;
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = "";


                con.Open();
                SqlCommand cmd;
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = con.BeginTransaction("SampleTransaction");
                //cmd.Connection = con;
                try
                {
                    // string query = "";
                    query = "Insert into tblDeployStatus(DSServer,MediaID,FormatID,IsExists,DateTime,DELFG,CRTDT,CRTCD,UserIPAddress) Values ";
                    query += "(@DSServer,@MediaID,@FormatID,@IsExists,@DateTime,@DELFG,@CRTDT,@CRTCD,@UserIPAddress);";
                    //query = "Select VideoId,Title,Description,Video,Thumbnail,FoderID,ConDuration from [tblVideoDemo] where FoderID =@FoderID order by VideoId desc";
                    cmd = new SqlCommand(query, con);
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@DSServer", _tblDeployStatus.DSServer);
                    cmd.Parameters.AddWithValue("@MediaID", _tblDeployStatus.MediaID);
                    cmd.Parameters.AddWithValue("@FormatID", _tblDeployStatus.FormatID);
                    cmd.Parameters.AddWithValue("@IsExists", _tblDeployStatus.IsExists);
                    cmd.Parameters.AddWithValue("@DateTime", _tblDeployStatus.DateTime);
                    cmd.Parameters.AddWithValue("@DELFG", _tblDeployStatus.DELFG);
                    cmd.Parameters.AddWithValue("@CRTCD", _tblDeployStatus.CRTCD);
                    cmd.Parameters.AddWithValue("@CRTDT", _tblDeployStatus.CRTDT);
                    cmd.Parameters.AddWithValue("@UserIPAddress", _tblDeployStatus.UserIPAddress);

                    cmd.CommandType = CommandType.Text;

                    result = cmd.ExecuteNonQuery();
                    //  CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);


                    query = "";
                    query = "Insert into tblDeployLog(Server,MediaID,FormatID,ElapsedTime,CopiedBytes,Result,DELFG,CRTDT,CRTCD,IPAddress,DateTime) Values ";
                    query += "(@Server,@MediaID,@FormatID,@ElapsedTime,@CopiedBytes,@Result,@DELFG,@CRTDT,@CRTCD,@IPAddress,@DateTime);";
                    //query = "Select VideoId,Title,Description,Video,Thumbnail,FoderID,ConDuration from [tblVideoDemo] where FoderID =@FoderID order by VideoId desc";
                    cmd = new SqlCommand(query, con);
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@Server", _tblDeployStatus.DSServer);
                    cmd.Parameters.AddWithValue("@MediaID", _tblDeployStatus.MediaID);
                    cmd.Parameters.AddWithValue("@FormatID", _tblDeployStatus.FormatID);
                    cmd.Parameters.AddWithValue("@ElapsedTime", Convert.ToInt32(_tblDeployStatus.Duration));
                    cmd.Parameters.AddWithValue("@CopiedBytes", Convert.ToInt32(_tblDeployStatus.FileSize));
                    cmd.Parameters.AddWithValue("@Result", _tblDeployStatus.Result);
                    cmd.Parameters.AddWithValue("@DELFG", _tblDeployStatus.DELFG);
                    cmd.Parameters.AddWithValue("@CRTCD", _tblDeployStatus.CRTCD);
                    cmd.Parameters.AddWithValue("@CRTDT", _tblDeployStatus.CRTDT);
                    cmd.Parameters.AddWithValue("@IPAddress", _tblDeployStatus.UserIPAddress);
                    cmd.Parameters.AddWithValue("@DateTime", _tblDeployStatus.CRTDT);

                    cmd.CommandType = CommandType.Text;

                    result = cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                    Log.Error(LogInfo.LogMsg, ex);
                }

            }



            return result;
        }
    }
}