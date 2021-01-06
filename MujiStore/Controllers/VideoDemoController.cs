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
    [MUJICustomAuthorize(Roles = "4,5,6,7,12,13,14,15,20,21,22,23,28,29,30,31")]

    public class VideoDemoController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();
        string CS = ConfigurationManager.ConnectionStrings["cnstr"].ConnectionString;
        int depth = 0;
        // GET: VideoDemo
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
            lstConvertStatus.Add(new SelectListItem { Text = "Completed", Value = "3" });//VJ 20200603 ConvertStatus Changed
            lstConvertStatus.Add(new SelectListItem { Text = "In Progress", Value = "0" });
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
                List<string> ffmt = new List<string>();
                ffmt = MujiStore.BLL.CommonLogic.GetFileExtn("video");
                List<BulkUploadVideo> VidList = new List<BulkUploadVideo>();
                var videoAllFiles = Directory.EnumerateFiles(Server.MapPath("~/FtpVideo"));
                //MediaSearch msearch = new MediaSearch();

                //ViewData["SearchMedia"] = msearch;
                var videoFiles = from selectfiles in videoAllFiles
                                 where (from filedtl in videoAllFiles
                                        from filextn in ffmt
                                        where filedtl.ToLower().EndsWith(filextn)
                                        select filedtl).Contains(selectfiles)
                                 select selectfiles;

                foreach (string file in videoFiles)
                {
                    BulkUploadVideo buMedia = new BulkUploadVideo();
                    buMedia.UploadFileName = Path.GetFileName(file);
                    buMedia.UploadTitle = string.Empty;
                    buMedia.UploadDescription = string.Empty;
                    buMedia.UploadFolderID = 1;
                    buMedia.IsUpload = false;
                    buMedia.IsDelete = false;
                    VidList.Add(buMedia);
                    // Console.WriteLine(Path.GetFileName(file));
                }
                FillApprovalStatus();
                ViewData["videoFiles"] = VidList;
                //When fils is exit, then need to show for the update. Count will send to Index Page
                var videoFilesCount = Directory.EnumerateFiles(Server.MapPath("~/FtpVideo")).Count();
                ViewBag.VideoFilesCount = VidList.Count;
                //Verify that file is convered
                var videoConvertedFiles = Directory.EnumerateFiles(Server.MapPath("~/ffmpeg/tmp"), "*.txt");
                var videoConvertedFilesSelect = from selectfiles in videoConvertedFiles
                                                select selectfiles;
                //By Default
                Boolean convertStatus = false;
                int convertedcount = 0;
                foreach (string outputResultFile in videoConvertedFilesSelect)
                {
                    convertedcount = convertedcount + 1;
                    var fileCheck = new FileInfo(outputResultFile);
                    if (IsFileLocked(fileCheck) == false) // If file is not locked then it can be delete
                    {
                        convertStatus = true;
                    }

                }
                //Check the converted count
                if (convertedcount == 0)
                {
                    //Verify that any batch file is available , hence the conversion process start with batch file
                    var batchCouunt = Directory.EnumerateFiles(Server.MapPath("~/ffmpeg/tmp"), "*.bat").Count();
                    if (batchCouunt > 0)
                    {
                        ViewBag.videoConvertedFilesCount = -99;
                    }
                    else
                    {
                        ViewBag.videoConvertedFilesCount = 0;
                    }
                }
                else
                {
                    if (convertStatus == true)
                    {
                        ViewBag.videoConvertedFilesCount = 1;
                    }
                    else
                    {
                        ViewBag.videoConvertedFilesCount = -99;
                    }

                }
                //var videoList="";
                List<tblMedia> videoList = new List<tblMedia>();
                //var videoList;
                string uName = Session["UserName"].ToString();
                ViewData["FolderDtl"] = BLL.CommonLogic.FillFolderList();
                string query = "select MediaID,Title,Description,Video,ConvertStatus,tblMedia.FolderID,Name ";
                query = query + "from [dbo].[tblMedia] left join tblFolder on tblMedia.FolderID = tblFolder.FolderID where ";
                query = query + " ApprovalStatus < 3 AND PhysicalDELFG = 0 ";
                query = query + " and FORMAT(tblMedia.CRTDT, 'yyyy-MM-dd') between '" + CdStDate + "' and '" + CdEdDate + "'";
                //query = query + " lastName like '%" + Name + "%' and EnrollmentDate ";
                if (Session["ApprovalFlag"].ToString() != "1")
                {
                    query = query + " and Registerer = '" + uName + "'";
                    query = query + " and tblMedia.DELFG = 0";
                }
                //else
                //{
                //    if (ViewBag.DFlag != null && ViewBag.DFlag != 2)
                //    {
                //        query = query + " and tblMedia.DELFG = " + ViewBag.DFlag;
                //    }
                //}

                if (ViewBag.SearTitle != null && ViewBag.SearTitle != "" && ViewBag.SearTitle != string.Empty)
                {
                    query = query + " and title like '%" + ViewBag.SearTitle + "%'";
                }

                if (ViewBag.cvtStatus != null && ViewBag.cvtStatus != 4)
                {
                    query = query + " and ConvertStatus = " + ViewBag.cvtStatus;
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
                                    FolderName = Convert.ToString(sdr["Name"])
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
                        videoList = videoList.OrderByDescending(o => o.ConvertStatus).ToList();
                        break;
                    case "ConvertStatus":
                        videoList = videoList.OrderBy(o => o.ConvertStatus).ToList();
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


                return View(VidList);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }




        }

        /// <summary>
        /// Update Video converted status
        /// </summary>
        /// <returns></returns>
        ///
        //[HttpPost]
        //public ActionResult Index(IEnumerable<BulkUploadVideo> _tblVideoDemo)
        //{
        //    MediaSearch m = new MediaSearch();
        //    m = ViewData["SearchMedia"] as MediaSearch;
        //   // Index(1);
        //    return View();
        //}
        public ActionResult UpdateVideoConvertedStatus()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {

                //Loading text file list
                var videoAllFiles = Directory.EnumerateFiles(Server.MapPath("~/ffmpeg/tmp"), "*.txt");
                //Select 
                var videoFiles = from selectfiles in videoAllFiles
                                 select selectfiles;

                String strUpdatedText = "";
                //Intialize fi
                System.IO.FileInfo fi = null;
                //Update and delete the file about conversion status
                foreach (string outputResultFile in videoFiles)
                {
                    //Read line
                    string strTextLine;
                    int durationIndex = -1;
                    int fileDuration = 0;
                    System.IO.StreamReader file = null;
                    //Assign text file from the list
                    string strFileName = Path.GetFileNameWithoutExtension(outputResultFile);
                    //Get extension
                    string strExt = Path.GetExtension(outputResultFile);
                    var fileCheck = new FileInfo(outputResultFile);
                    //Verify that file is locked (the file is read only mode or RW mode)
                    if (IsFileLocked(fileCheck) == false)
                    {
                        try
                        {
                            //Read from the text file
                            file = new System.IO.StreamReader(outputResultFile);
                            //Read the ile until reach the duration
                            while ((strTextLine = file.ReadLine()) != null)
                            {
                                //find the text of Duration
                                durationIndex = strTextLine.IndexOf("Duration: ");
                                if (durationIndex >= 0)
                                {
                                    //Reading File Duration and Size
                                    //Sample  Duration: 00:00:11.83, start: 0.000000, bitrate: 1011 kb/s
                                    string[] durationLine = strTextLine.Split(',');
                                    StringBuilder sbDuration = new StringBuilder(durationLine[0]); //0 Duration, 1 start 2 bitrate
                                    sbDuration.Replace("Duration: ", "");
                                    sbDuration.Replace(" ", "");
                                    sbDuration.Replace(".", ":");
                                    //Convert String;
                                    String strTmp = sbDuration.ToString();
                                    string[] durationDelimiter = strTmp.Split(':');
                                    // 0 hours 1 minutes 2 seconds 3 millisecond
                                    int intHours = Int32.Parse(durationDelimiter[0]);
                                    int intMinutes = Int32.Parse(durationDelimiter[1]);
                                    int intSeconds = Int32.Parse(durationDelimiter[2]);
                                    //Calculate the duration
                                    fileDuration = (intHours * 3600) + (intMinutes * 60) + intSeconds; //millisecond not included
                                    break;
                                }
                            }
                        }
                        catch { }
                        finally
                        {
                            file.Close();
                        }

                        //Reading for FileSize in  file itself
                        String strRootDirectory = Server.MapPath("~/");
                        fi = new FileInfo(strRootDirectory + @"\Video\" + strFileName + ".mp4");
                        // Get file size  
                        long fileSize = fi.Length / 1024;

                        //Update Status
                        tblMedia tblmed = new tblMedia();
                        tblmed.Video = strFileName + ".mp4";
                        tblmed.FileSize = fileSize;
                        tblmed.Duration = fileDuration;
                        tblmed.ConvertStatus = 3;  //0 Not Converted 1 Converted //VJ 20200603 ConvertStatus 3 Converted  Changed
                                                   //For Return to the user
                        strUpdatedText = strUpdatedText + strFileName + ".mp4, ";
                        //Update the status to the database 
                        int result = UpdateConvertedStatusMediaFile(tblmed);
                        //Delete File .txt File 
                        fi = new System.IO.FileInfo(outputResultFile);
                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                    }
                }
                //Remove comma
                string strUpdatedText1 = strUpdatedText.Remove(strUpdatedText.Length - 1, 1);
                strUpdatedText1 = " ( " + strUpdatedText1 + " ) ";
                TempData["SuccMsg"] = MujiStore.Resources.Resource.CntVideoDemoUpdateVCSSuccMsg + strUpdatedText1;
                return Redirect("Index");

            }
            catch (Exception ex)
            {
                //Write exception to log
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                //To know about the error to the user
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntVideoDemoUpdateVCSErrMsg;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        /// <summary>
        /// Verify that File is Read only / RW permission
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual bool IsFileLocked(FileInfo file)
        {
            //Initialze Filestream
            FileStream stream = null;
            try
            {
                //Open the file with RW, if error raise then the file is read mode.
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                //Close the stream
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
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
                querySDtnt +=   ") SServer ";
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
        /// <summary>
        /// Streaming video conversion
        /// </summary>
        /// <param name="_tblVideoDemo"></param>
        /// <returns></returns>
        /// 
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
        [HttpPost]
        public ActionResult CreateFtpvideo(IEnumerable<BulkUploadVideo> _tblVideoDemo)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                //Root Path
                String strRootDirectory = Server.MapPath("~/");
                string sourcePath = strRootDirectory + @"\FtpVideo";
                string targetPath = strRootDirectory + @"\Video";

                //Batch Processing Flag
                Boolean IsBatchProcesing = false;
                //Batch FileName
                string strBatchFile = DateTime.Now.ToString("yyyyMMddHHmmss") + ".bat";
                //Master Batch File
                FileInfo fileMasterBatch = new FileInfo(strRootDirectory + @"ffmpeg\tmp\" + strBatchFile);
                using (StreamWriter swMaster = fileMasterBatch.CreateText())
                {
                    foreach (var bupload in _tblVideoDemo)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(strRootDirectory + @"\FtpVideo\" + bupload.UploadFileName);
                        if (fi.Exists)
                        {
                            //If the checkbox (Delete) is selected, then delete the file from FTP folder
                            if (bupload.IsDelete == true)
                            {
                                fi.Delete();
                            }
                            else if (bupload.IsUpload == true)
                            {
                                //Verify that title should not empty
                                if (bupload.UploadTitle == null || bupload.UploadTitle == string.Empty || bupload.UploadTitle.Trim() == "")
                                {
                                    ViewBag.ErrMsg = MujiStore.Resources.Resource.CntVideoDemoCreateFtpvideoErrMsg1 + bupload.UploadFileName;

                                    //return Redirect("Index");
                                }
                                else
                                {
                                    //Assign true for batch processing 
                                    IsBatchProcesing = true;
                                    //Intialize Media class
                                    tblMedia tblmedia = new tblMedia();
                                    //Intialize File
                                    string sourceFile = System.IO.Path.Combine(sourcePath, bupload.UploadFileName);
                                    string ModFileName = CommonLogic.GetFileNameMp4(bupload.UploadFileName); //ModifiedFileName
                                    string destFile = System.IO.Path.Combine(targetPath, ModFileName);
                                    tblMedia tblmed = new tblMedia();
                                    tblmed.Title = bupload.UploadTitle;
                                    tblmed.Description = bupload.UploadDescription;
                                    tblmed.Video = ModFileName;
                                    tblmed.FolderID = bupload.UploadFolderID;
                                    tblmed.UploadType = "F";
                                    tblmed.FileSize = 0;
                                    tblmed.Duration = 0;
                                    tblmed.ConvertStatus = 0;  //0 Not Converted 3 Converted

                                    //Update video information to the database
                                    int result = InsertMediaFile(tblmed);
                                    //Write to log
                                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                                    //For Temporary File Handling
                                    string tmpFileName = Path.GetFileNameWithoutExtension(ModFileName);
                                    //Output conversion file
                                    string outputResultFile = strRootDirectory + @"ffmpeg\tmp\" + tmpFileName + ".txt";
                                    string fileNameBas = strRootDirectory + @"\ffmpeg\tmp\" + tmpFileName + ".bat";
                                    // Check if file already exists. If yes, delete it.     
                                    if (System.IO.File.Exists(fileNameBas))
                                    {
                                        System.IO.File.Delete(fileNameBas);
                                    }

                                    //Move Orginal file to Backup folder when web.config videosrccbackup=true
                                    string videosrccbackup = "";
                                    try
                                    {
                                        videosrccbackup = System.Configuration.ConfigurationManager.AppSettings["videosrccbackup"];
                                    }
                                    catch
                                    {

                                    }

                                    FileInfo fileFfmpeg = new FileInfo(fileNameBas);
                                    using (StreamWriter sw = fileFfmpeg.CreateText())
                                    {
                                        //Write to Master Batch File
                                        swMaster.WriteLine("call " + strRootDirectory + @"\ffmpeg\tmp\" + tmpFileName);

                                        //Setting Path
                                        sw.WriteLine("SET PATH=%PATH%;" + strRootDirectory + @"\ffmpeg\bin");
                                        //For file conversion to MP4
                                        sw.WriteLine("ffmpeg -i " + sourceFile + " -c:a aac -b:a 128k -c:v libx264 -crf 23 " + destFile + " 2>" + outputResultFile);

                                        if (videosrccbackup == "true")
                                        {
                                            //File move to Backup folder
                                            sw.WriteLine("move " + sourceFile + "  " + strRootDirectory + @"ffmpeg\tmp\backup\" + tmpFileName + Path.GetExtension(bupload.UploadFileName));
                                        }
                                        else
                                        {
                                            fi = new System.IO.FileInfo(sourceFile);
                                            if (fi.Exists)
                                            {
                                                //Delete the file when the flag is set backup as false
                                                sw.WriteLine("del " + sourceFile);
                                            }
                                        }
                                        // Finally delete the batch file once the execution is completed
                                        sw.WriteLine("del " + fileNameBas);
                                        sw.WriteLine("exit");
                                        sw.Close();
                                    }
                                }
                            }
                        }
                    }
                    //Finally the master batch file must delete from the system. 
                    swMaster.WriteLine("Del " + strRootDirectory + @"ffmpeg\tmp\" + strBatchFile);
                    swMaster.WriteLine("exit");
                    //Master Batch File Execution
                    swMaster.Close();
                    //*******************************************************
                    if (IsBatchProcesing == true)
                    {
                        //Intialize Thread class
                        ThreadProcess thProcess = new ThreadProcess();
                        //Assing Batch Task to thread                       
                        Thread job = new Thread(() => thProcess.ThreadTask(strRootDirectory + @"ffmpeg\tmp\" + strBatchFile));
                        //Start the batch job
                        job.Start();
                        //Inform to the user that video conversion is under process..
                        TempData["SuccMsg"] = MujiStore.Resources.Resource.CntVideoDemoCreateFtpvideoSuccMsg;
                    }
                    else
                    {
                        //No Batch Processing, delete the file
                        fileMasterBatch = new System.IO.FileInfo(strRootDirectory + @"ffmpeg\tmp\" + strBatchFile);
                        if (fileMasterBatch.Exists)
                        {
                            fileMasterBatch.Delete();
                        }

                    }

                }
                return Redirect("Index");
            }
            catch (Exception ex)
            {
                //Write exception to log
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                //To know about the error to the user
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntVideoDemoCreateFtpvideoErrMsg2;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
       


        //GET: VideoDemo/Details/5
        [NonAction]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblMedia tblVideoDemo = db.tblMedias.Find(id);
            if (tblVideoDemo == null)
            {
                return HttpNotFound();
            }
            return View(tblVideoDemo);
        }

      
        public ActionResult Create()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                List<SelectListItem> lstFolderName = new List<SelectListItem>();
                lstFolderName = BLL.CommonLogic.FillFolderList();
                //lstFolderName.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
                ViewBag.FolderList = new SelectList(lstFolderName, "Value", "Text");

                return View();
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }
       
        // POST: VideoDemo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblMedia _tblVideoDemo)
        {
            //Write to Log
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                //Populate Folder

                List<SelectListItem> lstFolderName = new List<SelectListItem>();
                lstFolderName = BLL.CommonLogic.FillFolderList();

                ViewBag.FolderList = new SelectList(lstFolderName, "Value", "Text");
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                var errors1 = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
                //Verify the model state
                if (ModelState.IsValid == false)
                {
                    return View(_tblVideoDemo);
                }
                //File must be selected
                if (_tblVideoDemo.PostedFile != null)
                {
                    //Root Path
                    String strRootDirectory = Server.MapPath("~/");
                    string sourcePath = strRootDirectory + @"\FtpVideo";
                    string targetPath = strRootDirectory + @"\Video";
                    //Original File Name conver to sequance file name
                    var fileName = CommonLogic.ModifiedFileName(_tblVideoDemo.PostedFile.FileName);
                    //To save file
                    var fileNameTmp = Path.Combine(Server.MapPath("~/FtpVideo"), fileName);
                    _tblVideoDemo.PostedFile.SaveAs(fileNameTmp);
                    //Batch Processing Flag
                    Boolean IsBatchProcesing = false;
                    //Batch FileName
                    string strBatchFile = DateTime.Now.ToString("yyyyMMddHHmmss") + ".bat";
                    //Master Batch File
                    FileInfo fileMasterBatch = new FileInfo(strRootDirectory + @"ffmpeg\tmp\" + strBatchFile);
                    using (StreamWriter swMaster = fileMasterBatch.CreateText())
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(strRootDirectory + @"\FtpVideo\" + fileName);
                        if (fi.Exists)
                        {
                            //Assign true for batch processing 
                            IsBatchProcesing = true;
                            //Intialize File
                            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);//System.IO.Path.Combine(sourcePath, bupload.UploadFileName);
                            string ModFileName = System.IO.Path.GetFileNameWithoutExtension(fileName) + ".mp4";//CommonLogic.GetFileNameMp4(fileName); //ModifiedFileName
                            string destFile = System.IO.Path.Combine(targetPath, ModFileName);
                            //Assign File name to object 
                            _tblVideoDemo.Video = ModFileName;
                            _tblVideoDemo.FileSize = 0;
                            _tblVideoDemo.Duration = 0;
                            _tblVideoDemo.ConvertStatus = 0;  //0 Not Converted 3 Converted
                            _tblVideoDemo.UploadType = "B"; //B - indicate Browser
                            //Update video information to the database
                            int result = InsertMediaFile(_tblVideoDemo);
                            //Write to log
                            CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                            //For Temporary File Handling
                            string tmpFileName = Path.GetFileNameWithoutExtension(ModFileName);
                            //Output conversion file
                            string outputResultFile = strRootDirectory + @"ffmpeg\tmp\" + tmpFileName + ".txt";
                            string fileNameBas = strRootDirectory + @"\ffmpeg\tmp\" + tmpFileName + ".bat";
                            // Check if file already exists. If yes, delete it.     
                            if (System.IO.File.Exists(fileNameBas))
                            {
                                System.IO.File.Delete(fileNameBas);
                            }

                            //Move Orginal file to Backup folder when web.config videosrccbackup=true
                            string videosrccbackup = "";
                            try
                            {
                                videosrccbackup = System.Configuration.ConfigurationManager.AppSettings["videosrccbackup"];
                            }
                            catch
                            {

                            }
                            FileInfo fileFfmpeg = new FileInfo(fileNameBas);
                            using (StreamWriter sw = fileFfmpeg.CreateText())
                            {
                                //Write to Master Batch File
                                swMaster.WriteLine("call " + strRootDirectory + @"\ffmpeg\tmp\" + tmpFileName);
                                //Setting Path
                                sw.WriteLine("SET PATH=%PATH%;" + strRootDirectory + @"\ffmpeg\bin");
                                //For file conversion to MP4
                                sw.WriteLine("ffmpeg -i " + sourceFile + " -c:a aac -b:a 128k -c:v libx264 -crf 23 " + destFile + " 2>" + outputResultFile);

                                if (videosrccbackup == "true")
                                {
                                    //File move to Backup folder
                                    sw.WriteLine("move " + sourceFile + "  " + strRootDirectory + @"ffmpeg\tmp\backup\" + tmpFileName + Path.GetExtension(fileName));
                                }
                                else
                                {
                                    fi = new System.IO.FileInfo(sourceFile);
                                    if (fi.Exists)
                                    {
                                        //Delete the file when the flag is set backup as false
                                        sw.WriteLine("del " + sourceFile);
                                    }
                                }
                                // Finally delete the batch file once the execution is completed
                                sw.WriteLine("del " + fileNameBas);
                                sw.WriteLine("exit");
                                sw.Close();
                            }
                        }
                        //Finally the master batch file must delete from the system. 
                        swMaster.WriteLine("Del " + strRootDirectory + @"ffmpeg\tmp\" + strBatchFile);
                        swMaster.WriteLine("exit");
                        //Master Batch File Execution
                        swMaster.Close();
                        //*******************************************************
                        if (IsBatchProcesing == true)
                        {
                            //Intialize Thread class
                            ThreadProcess thProcess = new ThreadProcess();
                            //Assing Batch Task to thread    
                            Thread job = new Thread(() => thProcess.ThreadTask(strRootDirectory + @"ffmpeg\tmp\" + strBatchFile));
                            //Start the batch job
                            job.Start();
                            //Inform to the user that video conversion is under process..
                            TempData["SuccMsg"] = MujiStore.Resources.Resource.CntVideoDemoCreateSuccMsg;
                        }
                        else
                        {
                            //No Batch Processing, delete the file
                            fileMasterBatch = new System.IO.FileInfo(strRootDirectory + @"ffmpeg\tmp\" + strBatchFile);
                            if (fileMasterBatch.Exists)
                            {
                                fileMasterBatch.Delete();
                            }

                        }

                    }
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntVideoDemoCreateSuccMsg;
                    return RedirectToAction("Index");
                }
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntVideoDemoCreateErrMsg;
                return View(_tblVideoDemo);
            }

            catch (Exception ex)
            {
                //Write exception to log
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                //To know about the error to the user
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntVideoDemoCreateErrMsg;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create1(tblMedia _tblVideoDemo)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                // PopulateFolder();
                List<SelectListItem> lstFolderName = new List<SelectListItem>();
                lstFolderName = BLL.CommonLogic.FillFolderList();

                ViewBag.FolderList = new SelectList(lstFolderName, "Value", "Text");
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                var errors1 = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
                if (ModelState.IsValid == false)
                {
                    return View(_tblVideoDemo);
                }
                if (_tblVideoDemo.PostedFile != null)
                {
                    //Original File Name
                    var fileName = CommonLogic.ModifiedFileName(_tblVideoDemo.PostedFile.FileName);
                    //Temporary 
                    var fileNameTmp = Path.Combine(Server.MapPath("~/FtpVideo"), fileName);
                    LogInfo.Comments = "Video Created - " + _tblVideoDemo.Title.ToString();
                    //For store uploaded file 
                    _tblVideoDemo.PostedFile.SaveAs(fileNameTmp);
                    //*****************************************
                    //File Conversion to MP4
                    //Begin
                    String strRootDirectory = Server.MapPath("~/");
                    string sourcePath = strRootDirectory + @"\FtpVideo";
                    string targetPath = strRootDirectory + @"\Video";
                    string sourceFile = fileNameTmp;//System.IO.Path.Combine(sourcePath, fileName);
                    string ModFileName = CommonLogic.GetFileNameActualMp4(fileName); //ModifiedFileName
                    string destFile = System.IO.Path.Combine(targetPath, ModFileName);
                    //For Temporary File Handling
                    string tmpFileName = ModFileName;
                    tmpFileName = tmpFileName.Replace(".", ""); //Remove . with extension
                    //Output conversion file
                    string outputResultFile = strRootDirectory + @"ffmpeg\tmp\" + tmpFileName + ".txt";
                    string fileNameBas = strRootDirectory + @"\ffmpeg\tmp\" + tmpFileName + ".bat";
                    // Check if file already exists. If yes, delete it.     
                    if (System.IO.File.Exists(fileNameBas))
                    {
                        System.IO.File.Delete(fileNameBas);
                    }
                    FileInfo fileFfmpeg = new FileInfo(fileNameBas);
                    using (StreamWriter sw = fileFfmpeg.CreateText())
                    {
                        //Setting Path
                        sw.WriteLine("SET PATH=%PATH%;" + strRootDirectory + @"\ffmpeg\bin");
                        //For file conversion to MP4
                        sw.WriteLine("ffmpeg -i " + sourceFile + " -c:a aac -b:a 128k -c:v libx264 -crf 23 " + destFile + " 2>" + outputResultFile);
                    }
                    System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(fileNameBas);
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo = p;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();
                    proc.WaitForExit();
                    //Batch File End

                    //Reading File Duration and Size
                    //Sample  Duration: 00:00:11.83, start: 0.000000, bitrate: 1011 kb/s
                    string strTextLine;
                    int durationIndex = -1;
                    int fileDuration = 0;
                    System.IO.StreamReader file = null;
                    try
                    {
                        file = new System.IO.StreamReader(@outputResultFile);
                        while ((strTextLine = file.ReadLine()) != null)
                        {
                            durationIndex = strTextLine.IndexOf("Duration: ");
                            if (durationIndex >= 0)
                            {
                                string[] durationLine = strTextLine.Split(',');
                                StringBuilder sbDuration = new StringBuilder(durationLine[0]); //0 Duration, 1 start 2 bitrate
                                sbDuration.Replace("Duration: ", "");
                                sbDuration.Replace(" ", "");
                                sbDuration.Replace(".", ":");
                                //Convert String;
                                String strTmp = sbDuration.ToString();
                                string[] durationDelimiter = strTmp.Split(':');
                                // 0 hours 1 minutes 2 seconds 3 millisecond
                                int intHours = Int32.Parse(durationDelimiter[0]);
                                int intMinutes = Int32.Parse(durationDelimiter[1]);
                                int intSeconds = Int32.Parse(durationDelimiter[2]);
                                fileDuration = (intHours * 3600) + (intMinutes * 60) + intSeconds; //millisecond not included
                                break;
                            }
                        }
                    }
                    catch { }
                    finally
                    {
                        file.Close();
                    }

                    //Reading for FileSize in  file itself
                    System.IO.FileInfo fi;
                    fi = new FileInfo(destFile);
                    // Get file size  
                    long fileSize = fi.Length / 1024;

                    //Move Orginal file to Backup folder when web.config videosrccbackup=true
                    string videosrccbackup = "";
                    try
                    {
                        videosrccbackup = System.Configuration.ConfigurationManager.AppSettings["videosrccbackup"];
                    }
                    catch
                    {

                    }
                    if (videosrccbackup == "true")
                    {
                        System.IO.File.Move(sourceFile, strRootDirectory + @"ffmpeg\tmp\backup\" + fileName);
                    }
                    else
                    {
                        fi = new System.IO.FileInfo(sourceFile);
                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                    }
                    //Delete Temporary Handled File
                    //Output File
                    fi = new System.IO.FileInfo(outputResultFile);
                    if (fi.Exists)
                    {
                        fi.Delete();
                    }
                    //Batch File Delete
                    fi = new System.IO.FileInfo(fileNameBas);
                    if (fi.Exists)
                    {
                        fi.Delete();
                    }
                    //End of File Conversion
                    //*****************************************

                    _tblVideoDemo.Video = ModFileName;
                    _tblVideoDemo.FileSize = fileSize;
                    _tblVideoDemo.Duration = fileDuration;
                    _tblVideoDemo.ConvertStatus = 3;  //0 Not Converted 3 Converted //VJ 20200603 ConvertStatus 3 Converted  Changed
                    _tblVideoDemo.UploadType = "B";
                    //Comment on 20200331
                    int result = InsertMediaFile(_tblVideoDemo);
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntVideoDemoCreate1SuccMsg;
                    return RedirectToAction("Index");
                }
                //}





                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntVideoDemoCreateErrMsg;
                return View(_tblVideoDemo);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntVideoDemoCreateErrMsg;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }
      

        /// <summary>
        /// Update about filesize, duration and video conversion status
        /// </summary>
        /// <param name="_tblVideoDemo"></param>
        /// <returns></returns>
        public int UpdateConvertedStatusMediaFile(tblMedia _tblVideoDemo)
        {
            int result;
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = "";
                con.Open();
                SqlCommand cmd;
                tblMedia tblmda = new tblMedia();
                tblmda = db.tblMedias.Where(x => x.Video == _tblVideoDemo.Video).SingleOrDefault();

                //Assing Query String
                query = "UPDATE tblMedia SET FileSize=@FileSize,ConvertStatus=@ConvertStatus,Duration=@Duration where Video=@Video; ";
                //Intialize Command and pass the paramters
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Video", _tblVideoDemo.Video);
                cmd.Parameters.AddWithValue("@FileSize", _tblVideoDemo.FileSize);
                cmd.Parameters.AddWithValue("@ConvertStatus", _tblVideoDemo.ConvertStatus);
                cmd.Parameters.AddWithValue("@Duration", _tblVideoDemo.Duration);
                cmd.CommandType = CommandType.Text;
                result = cmd.ExecuteNonQuery();

                query = "";
                query = "UPDATE tblDeployLog SET CopiedBytes=@FileSize,ElapsedTime=@Duration where MediaID=@MediaID; ";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MediaID", tblmda.MediaID);
                cmd.Parameters.AddWithValue("@FileSize", _tblVideoDemo.FileSize);
                cmd.Parameters.AddWithValue("@Duration", _tblVideoDemo.Duration);
                cmd.CommandType = CommandType.Text;
                result = cmd.ExecuteNonQuery();

                query = "";
                query = "UPDATE tblMediaFormatInfo SET FileSize=@FileSize where MediaID=@MediaID; ";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MediaID", tblmda.MediaID);
                cmd.Parameters.AddWithValue("@FileSize", _tblVideoDemo.FileSize);
                cmd.CommandType = CommandType.Text;
                result = cmd.ExecuteNonQuery();
                //  CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
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
                    cmd.Parameters.AddWithValue("@ElapsedTime",Convert.ToInt32(_tblDeployStatus.Duration));
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
                catch(Exception ex)
                {
                    transaction.Rollback();
                    LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                    Log.Error(LogInfo.LogMsg, ex);
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
                catch(Exception ex)
                {
                    transaction.Rollback();
                    LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                    Log.Error(LogInfo.LogMsg, ex);
                }
            }
  
            return result;
        }
        public int InsertMediaFile(tblMedia _tblVideoDemo)
        {
            int result = 0;
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = "";
                query = "Insert into tblMedia(Title,Description,Video,IpAddress,FolderID,DELFG,CRTDT,CRTCD,UploadType,ViewCount,PhysicalDELFG,FileSize,Registerer,ApprovalStatus,ConvertStatus,Duration) Values ";
                query += "(@Title,@Description,@Video,@IpAddress,@FolderID,@DELFG,@CRTDT,@CRTCD,@UploadType,@ViewCount,@PhysicalDELFG,@FileSize,@Registerer,@ApprovalStatus,@ConvertStatus,@Duration); SELECT SCOPE_IDENTITY();";
                //query = "Select VideoId,Title,Description,Video,Thumbnail,FoderID,ConDuration from [tblVideoDemo] where FoderID =@FoderID order by VideoId desc";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Title", _tblVideoDemo.Title);
                cmd.Parameters.AddWithValue("@Description", _tblVideoDemo.Description);
                cmd.Parameters.AddWithValue("@Video", _tblVideoDemo.Video);
                cmd.Parameters.AddWithValue("@IpAddress", Session["IPAddress"].ToString());
                cmd.Parameters.AddWithValue("@FolderID", _tblVideoDemo.FolderID);
                cmd.Parameters.AddWithValue("@DELFG", false);
                cmd.Parameters.AddWithValue("@CRTCD", Session["UserName"].ToString());
                cmd.Parameters.AddWithValue("@CRTDT", DateTime.Now);
                cmd.Parameters.AddWithValue("@UploadType", _tblVideoDemo.UploadType);
                cmd.Parameters.AddWithValue("@ViewCount", 0);
                cmd.Parameters.AddWithValue("@PhysicalDELFG", false);
                cmd.Parameters.AddWithValue("@FileSize", _tblVideoDemo.FileSize);
                cmd.Parameters.AddWithValue("@Registerer", Session["UserName"].ToString());
                cmd.Parameters.AddWithValue("@ApprovalStatus", 0);
                cmd.Parameters.AddWithValue("@ConvertStatus", _tblVideoDemo.ConvertStatus);
                cmd.Parameters.AddWithValue("@Duration", _tblVideoDemo.Duration);
                cmd.CommandType = CommandType.Text;
                con.Open();
                //result = cmd.ExecuteNonQuery();
                //int medid = (int)cmd.ExecuteScalar();
                int modified = Convert.ToInt32(cmd.ExecuteScalar());

                tblMedia med = new tblMedia();
                med = db.tblMedias.Where(x => x.MediaID == modified).SingleOrDefault();

                string fileName = med.Video;
                FileInfo fi = new FileInfo(fileName);
                string extn = fi.Extension.TrimStart('.');

                tblFormat formt = new tblFormat();
                formt = db.tblFormats.Where(x => x.Name.ToUpper() == extn.ToUpper()).SingleOrDefault();

                tblMediaFormatInfo mfinfo = new tblMediaFormatInfo();
                mfinfo.MediaID = modified;
                mfinfo.FormatID = formt.FormatID;
                mfinfo.FileSize = _tblVideoDemo.FileSize;
                mfinfo.DELFG = true;
                mfinfo.CRTCD = Session["UserName"].ToString();
                mfinfo.CRTDT = DateTime.Now;
                mfinfo.IPAddress = Session["IPAddress"].ToString();
                db.tblMediaFormatInfoes.Add(mfinfo);
                db.SaveChanges();
                //  CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);


            }


            return result;
        }

        public int InsertMediaApplication(tblMedia _tblVideoDemo, bool ImageUpd)
        {
            int result = 0;
            try
            {
               
                string query = "";
                using (SqlConnection con = new SqlConnection(CS))
                {
                    if (ImageUpd == true)
                    {
                        query = "  Update tblMedia set Thumbnail=@Thumbnail,IpAddress=@IpAddress,UPDDT=@UPDDT,UPDCD=@UPDCD";
                        query += " Where MediaID=@MediaID";

                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Thumbnail", _tblVideoDemo.Thumbnail== null ? "" : _tblVideoDemo.Thumbnail);
                        cmd.Parameters.AddWithValue("@UPDDT", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UPDCD", Session["UserName"].ToString());
                        cmd.Parameters.AddWithValue("@IpAddress", Session["IPAddress"].ToString());
                        cmd.Parameters.AddWithValue("@MediaID", _tblVideoDemo.MediaID);
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        result = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                        query = "";
                        query = "Insert into tblApplication(MediaID,Title,Description,FolderID,NewApprovalStatus,[Delete],Registerer,Memo,RegisteredDate,DELFG,CRTDT,CRTCD,IPAddress) Values ";
                        query += "(@MediaID,@Title,@Description,@FolderID,@ApprovalStatus,@PhysicalDELFG,@Registerer,@Memo,@RegisteredDate,@DELFG,@CRTDT,@CRTCD,@IpAddress); SELECT SCOPE_IDENTITY();";
                        //query = "Select VideoId,Title,Description,Video,Thumbnail,FoderID,ConDuration from [tblVideoDemo] where FoderID =@FoderID order by VideoId desc";
                        SqlCommand cmdApp = new SqlCommand(query, con);
                        cmdApp.Parameters.AddWithValue("@MediaID", _tblVideoDemo.MediaID);
                        cmdApp.Parameters.AddWithValue("@Title", _tblVideoDemo.Title);
                        cmdApp.Parameters.AddWithValue("@Description", _tblVideoDemo.Description);
                        cmdApp.Parameters.AddWithValue("@FolderID", _tblVideoDemo.FolderID);
                        cmdApp.Parameters.AddWithValue("@ApprovalStatus", _tblVideoDemo.ApprovalStatus);
                        cmdApp.Parameters.AddWithValue("@PhysicalDELFG", _tblVideoDemo.PhysicalDELFG);
                        cmdApp.Parameters.AddWithValue("@Registerer", Session["UserName"].ToString());
                    //cmdApp.Parameters.AddWithValue("@Memo", _tblVideoDemo.Comments);
                        cmdApp.Parameters.AddWithValue("@Memo", _tblVideoDemo.Comments == null ? "" : _tblVideoDemo.Comments);
                        cmdApp.Parameters.AddWithValue("@RegisteredDate", DateTime.Now);
                        cmdApp.Parameters.AddWithValue("@DELFG", false);
                        cmdApp.Parameters.AddWithValue("@CRTCD", Session["UserName"].ToString());
                        cmdApp.Parameters.AddWithValue("@CRTDT", DateTime.Now);
                        cmdApp.Parameters.AddWithValue("@IpAddress", Session["IPAddress"].ToString());
                        cmdApp.CommandType = CommandType.Text;
                        con.Open();
                        //result = cmd.ExecuteNonQuery();
                        //int medid = (int)cmd.ExecuteScalar();
                        result = Convert.ToInt32(cmdApp.ExecuteScalar());


                    
                }

            }
            catch(Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntVideoDemoSaveThumbnailImageErrMsg;
              
            }


                return result;
        }

        public int UpdateMediaFile([Bind(Exclude = "PostedFile")] tblMedia _tblVideoDemo, bool PhyDelFlag, bool ImageUpd)
        {
            int result;
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = "";
                query = "Update tblMedia set Title=@Title,Description=@Description,FolderID=@FolderID,";
                query += "DELFG=@DELFG,PhysicalDELFG=@PhysicalDELFG";
                if (PhyDelFlag == true)
                {
                    query += ",PhysicalDELFGCRTDT=@PhysicalDELFGCRTDT,PhysicalDELFGCRTCD=@PhysicalDELFGCRTCD,PhysicalDELIpAddress=@PhysicalDELIpAddress";
                }
                if (PhyDelFlag == false)
                {
                    query += ",IpAddress=@IpAddress,UPDDT=@UPDDT,UPDCD=@UPDCD";
                }
                if (ImageUpd == true)
                {
                    query += ",Thumbnail=@Thumbnail";
                }
                if (Session["ApprovalFlag"].ToString() == "1")
                {
                    query += ",Accepter=@Accepter,ApprovalStatus=@ApprovalStatus";
                }


                //,PhysicalDELFGCRTDT=@PhysicalDELFGCRTDT,";
                //query = "PhysicalDELFGCRTCD=@PhysicalDELFGCRTCD,PhysicalDELIpAddress=@PhysicalDELIpAddress";
                query += " Where MediaID=@MediaID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Title", _tblVideoDemo.Title);
                cmd.Parameters.AddWithValue("@Description", _tblVideoDemo.Description);
                cmd.Parameters.AddWithValue("@FolderID", _tblVideoDemo.FolderID);
                cmd.Parameters.AddWithValue("@DELFG", _tblVideoDemo.DELFG);
                cmd.Parameters.AddWithValue("@PhysicalDELFG", _tblVideoDemo.PhysicalDELFG);
                if (PhyDelFlag == true)
                {
                    cmd.Parameters.AddWithValue("@PhysicalDELFGCRTDT", DateTime.Now);
                    cmd.Parameters.AddWithValue("@PhysicalDELFGCRTCD", Session["UserName"].ToString());
                    cmd.Parameters.AddWithValue("@PhysicalDELIpAddress", Session["IPAddress"].ToString());
                }
                if (PhyDelFlag == false)
                {
                    cmd.Parameters.AddWithValue("@UPDDT", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UPDCD", Session["UserName"].ToString());
                    cmd.Parameters.AddWithValue("@IpAddress", Session["IPAddress"].ToString());
                }
                if (ImageUpd == true)
                {
                    cmd.Parameters.AddWithValue("@Thumbnail", _tblVideoDemo.Thumbnail);
                }
                if (Session["ApprovalFlag"].ToString() == "1")
                {
                    cmd.Parameters.AddWithValue("@Accepter", Session["UserName"].ToString());
                    cmd.Parameters.AddWithValue("@ApprovalStatus", _tblVideoDemo.ApprovalStatus);
                }

                cmd.Parameters.AddWithValue("@MediaID", _tblVideoDemo.MediaID);
                cmd.CommandType = CommandType.Text;
                con.Open();
                result = cmd.ExecuteNonQuery();

                tblMediaFormatInfo mfinfo = new tblMediaFormatInfo();
                mfinfo.MediaID = _tblVideoDemo.MediaID;
               
                
                if (PhyDelFlag == true)
                {
                    mfinfo.DELFG = true;
                }
                else
                {
                    mfinfo.DELFG = _tblVideoDemo.DELFG;
                }
                query = "";
                query = " Update tblMediaFormatInfo set ";
                query += " DELFG=@DELFG,IpAddress=@IpAddress,UPDDT=@UPDDT,UPDCD=@UPDCD ";
                query += " Where MediaID=@MediaID";
                SqlCommand cmdMedifmt = new SqlCommand(query, con);
                cmdMedifmt.Parameters.AddWithValue("@DELFG", mfinfo.DELFG);
                cmdMedifmt.Parameters.AddWithValue("@UPDDT", DateTime.Now);
                cmdMedifmt.Parameters.AddWithValue("@UPDCD", Session["UserName"].ToString());
                cmdMedifmt.Parameters.AddWithValue("@IpAddress", Session["IPAddress"].ToString());
                cmdMedifmt.Parameters.AddWithValue("@MediaID", _tblVideoDemo.MediaID);
                cmdMedifmt.CommandType = CommandType.Text;
                result = cmdMedifmt.ExecuteNonQuery();

                //mfinfo.UPDCD = Session["UserName"].ToString();
                //mfinfo.UPDDT = DateTime.Now;
                //mfinfo.IPAddress = Session["IPAddress"].ToString();
                //db.Entry(mfinfo).State = EntityState.Modified;
                //db.SaveChanges();
            }
            return result;
        }
        [HttpPost]
        public ActionResult SaveThumbnailImage(tblMedia _tblVideoDemo)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            int result;
            try
            {
                FillApprovalStatus();
                List<SelectListItem> lstFolderName = new List<SelectListItem>();
                lstFolderName = BLL.CommonLogic.FillFolderList();
                lstFolderName.Insert(0, (new SelectListItem { Text = "Select", Value = "0" }));
                ViewData["FolderDtl"] = lstFolderName;

                List<SelectListItem> lstRobocopyExitcodeList = new List<SelectListItem>();
                ViewData["roboResult"] = FillRobocopyExitcodeList();
               // ViewData["MediaServer"] = GetMediaServerIPAddress(_tblVideoDemo.MediaID);
                if (_tblVideoDemo.FolderID == 0)
                {
                    ModelState.AddModelError("FolderID", "Select Folder");
                    return View("Edit", _tblVideoDemo);
                }

                var fileName = "";
                db.Entry(_tblVideoDemo).State = EntityState.Modified;
                if (_tblVideoDemo.PhysicalDELFG == true)
                {
                    /* Physical Deletion commented VJ 20200605
                    String strRootDirectory = Server.MapPath("~/");
                    string targetPath = strRootDirectory + @"\Video\";
                    if (_tblVideoDemo.Video != null && _tblVideoDemo.Video.Trim() != "" && _tblVideoDemo.Video.Trim().Length > 4)
                    {
                        System.IO.FileInfo fiV = new System.IO.FileInfo(targetPath + _tblVideoDemo.Video);
                        if (fiV.Exists)
                        {
                            fiV.Delete();
                        }
                    }
                    fileName = _tblVideoDemo.Video.ToLower().Replace(".mp4", ".png");
                    if (fileName != null && fileName.Trim() != "" && fileName.Trim().Length > 4)
                    {
                        System.IO.FileInfo fiV = new System.IO.FileInfo(targetPath + fileName);
                        if (fiV.Exists)
                        {
                            fiV.Delete();
                        }
                    }
                    Physical Deletion commented VJ 20200605 */
                    _tblVideoDemo.Thumbnail = _tblVideoDemo.ThumbnailFileName;
                    // result = UpdateMediaFile(_tblVideoDemo, true, true)
                    result = InsertMediaApplication(_tblVideoDemo, true);

                }
                else
                {
                    //path of folder taht you want to save the canvas
                    var path = Server.MapPath("~/Video");
                    //var path = Server.MapPath("http://" + ViewData["MediaServer"].ToString() + "/Media");
                    //produce new file name
                    fileName = _tblVideoDemo.Video.ToLower().Replace(".mp4", ".png");
                    //CommonLogic.ModifiedFileName(_tblVideoDemo.Video.ToLower().Replace(".mp4".ToLower(), ".png"));

                    //get full file path
                    var fileNameWitPath = Path.Combine(path, fileName);

                    //save canvas
                    using (var fs = new FileStream(fileNameWitPath, FileMode.Create))
                    {
                        using (var bw = new BinaryWriter(fs))
                        {
                            var data = Convert.FromBase64String(_tblVideoDemo.Thumbnail);
                            bw.Write(data);
                            bw.Close();
                        }
                        fs.Close();
                    }

                    _tblVideoDemo.Thumbnail = fileName;
                    //result = UpdateMediaFile(_tblVideoDemo, false, true);
                    result = InsertMediaApplication(_tblVideoDemo, true);
                    //_tblVideoDemo.UPDDT = DateTime.Now;
                    //_tblVideoDemo.UPDCD = Session["UserName"].ToString();
                    //_tblVideoDemo.IpAddress = Session["IPAddress"].ToString();

                }
            
                  
               
                //if (Session["ApprovalFlag"].ToString() == "2")
                //{
                //    if (_tblVideoDemo.deployStatus != null && _tblVideoDemo.PhysicalDELFG == false  && _tblVideoDemo.deployStatus.Count() > 0)
                //    {
                //        int i = 0;
                //        foreach (var bupload in _tblVideoDemo.deployStatus)
                //        {
                //            if(bupload.DELFG == true && (bupload.Result == null || bupload.Result == 0))
                //            {
                //                ModelState.AddModelError("_tblVideoDemo.deployStatus["+ i +"].Result", MujiStore.Resources.Resource.SelectRobocopyExitcodeContent);
                //                return View("Edit", _tblVideoDemo);
                //            }
                //            i += 1;
                //        }
                //        result =   UpdateDeployStatusAll(_tblVideoDemo.deployStatus);
                //    }
                    
                //}

                LogInfo.Comments = "Thumbnail and Details Updated - " + _tblVideoDemo.Title.ToString();
                CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                //db.SaveChanges();
                TempData["SuccMsg"] = MujiStore.Resources.Resource.CntVideoDemoSaveThumbnailImageSuccMsg;
                //do somthing with model
                return RedirectToAction("Index", "VideoDemo");
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntVideoDemoSaveThumbnailImageErrMsg;
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

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
                FillApprovalStatus();
                List<SelectListItem> lstFolderName = new List<SelectListItem>();
                lstFolderName = BLL.CommonLogic.FillFolderList();
                lstFolderName.Insert(0, (new SelectListItem { Text = "Select", Value = "0" }));
                ViewData["FolderDtl"] = lstFolderName;

                List<SelectListItem> lstRobocopyExitcodeList = new List<SelectListItem>();
                ViewData["roboResult"] = FillRobocopyExitcodeList();

                var fileName = "";

                if (_tblVideoDemo.FolderID == 0)
                {
                    ModelState.AddModelError("FolderID", "Select Folder");
                    return View("Edit", _tblVideoDemo);
                }
                db.Entry(_tblVideoDemo).State = EntityState.Modified;
                if (_tblVideoDemo.PhysicalDELFG == true)
                {
                    /* Physical Deletion commented VJ 20200605
                    String strRootDirectory = Server.MapPath("~/");
                    string targetPath = strRootDirectory + @"\Video\";
                    if (_tblVideoDemo.Video != null && _tblVideoDemo.Video.Trim() != "" && _tblVideoDemo.Video.Trim().Length > 4)
                    {
                        System.IO.FileInfo fiV = new System.IO.FileInfo(targetPath + _tblVideoDemo.Video);
                        if (fiV.Exists)
                        {
                            fiV.Delete();
                        }
                    }
                    fileName = _tblVideoDemo.Video.ToLower().Replace(".mp4", ".png");
                    if (fileName != null && fileName.Trim() != "" && fileName.Trim().Length > 4)
                    {
                        System.IO.FileInfo fiV = new System.IO.FileInfo(targetPath + fileName);
                        if (fiV.Exists)
                        {
                            fiV.Delete();
                        }
                    }
                    Physical Deletion commented VJ 20200605 */
                   // result = UpdateMediaFile(_tblVideoDemo, true, false);
                    result = InsertMediaApplication(_tblVideoDemo, false);
                }
                else
                {
                    //result = UpdateMediaFile(_tblVideoDemo, false, false);
                    result = InsertMediaApplication(_tblVideoDemo, false);
                }

                //if (Session["ApprovalFlag"].ToString() == "2")
                //{
                //    if (_tblVideoDemo.deployStatus != null && _tblVideoDemo.PhysicalDELFG == false  && _tblVideoDemo.deployStatus.Count() > 0)
                //    {
                //        int i = 0;
                //        foreach (var bupload in _tblVideoDemo.deployStatus)
                //        {
                //            if (bupload.DELFG == true && (bupload.Result == null || bupload.Result == 0))
                //            {
                //                ModelState.AddModelError("deployStatus[" + i + "].Result", MujiStore.Resources.Resource.SelectRobocopyExitcodeContent);
                //                return View("Edit", _tblVideoDemo);
                //            }
                //            i += 1;
                //        }
                //        result = UpdateDeployStatusAll(_tblVideoDemo.deployStatus);
                //    }

                //}
                LogInfo.Comments = "Video Details Updated - " + _tblVideoDemo.Title.ToString();
                CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                //db.SaveChanges();
                TempData["SuccMsg"] = MujiStore.Resources.Resource.CntVideoDemoSaveSaveVideoDetailsSuccMsg;
                //do somthing with model
                return RedirectToAction("Index", "VideoDemo");
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = MujiStore.Resources.Resource.CntVideoDemoSaveSaveVideoDetailsErrMsg;
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

        public string GetMediaServerIPAddress(int? MediaID)
        {
            string query = "";
            string result = "";
            using (SqlConnection con = new SqlConnection(CS))
            {
                //SqlCommand cmd = new SqlCommand("Select VideoId,Title,Description,Video,Thumbnail,FoderID from [tblVideoDemo]", con);
                query = "SELECT TOP 1 SS.IPAddress FROM [tblDeployStatus] AS DS ";
                query += " LEFT JOIN tblStreamServer AS SS ON DS.DSServer = SS.SSServer ";
                query += " LEFT JOIN tblFormat MF ON MF.FormatID = DS.FormatID ";
                query += " WHERE DS.DSServer = @DSServer  ";
                query += " AND DS.MediaID = @MediaID AND DS.DELFG = 0 ";
                query += " AND MF.Name = 'MP4' AND SS.DELFG = 0 ";
                query += " AND DS.IsExists = 1 AND MF.DELFG = 0 ";

                SqlCommand cmds = new SqlCommand(query, con);
                cmds.Parameters.AddWithValue("@MediaID", MediaID);
                cmds.Parameters.AddWithValue("@DSServer", System.Configuration.ConfigurationManager.AppSettings["MediaServer"]);
                cmds.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdrs = cmds.ExecuteReader();
                while (rdrs.Read())
                {
                    result = rdrs["IPAddress"].ToString();
                }
            }
            return result;
        }
        //public List<SelectListItem> FillList()
        //{
        //    string query = "";
        //    var list = new List<SelectListItem>();
        //    using (SqlConnection con = new SqlConnection(CS))
        //    {
        //        //SqlCommand cmd = new SqlCommand("Select VideoId,Title,Description,Video,Thumbnail,FoderID from [tblVideoDemo]", con);
        //        // query = "Select FolderID,Name from tblFolder where Delfg = 0 Order by FolderID";
        //        //query = ";WITH x AS ";
        //        //query += "(";
        //        //query += " SELECT FolderID, Name, ParentID, [level] = 1 ";
        //        //query += " FROM tblFolder WHERE ParentID = -1 AND   DELFG=0 ";
        //        //query += " UNION ALL ";
        //        //query += " SELECT t.FolderID, t.Name, t.ParentID, [level] = x.[level] + 1 ";
        //        //query += " FROM x INNER JOIN dbo.tblFolder AS t ";
        //        //query += " ON t.ParentID = x.FolderID ";
        //        //query += " ) ";
        //        //query += " SELECT FolderID, name,REPLICATE('  ', [level] - 1) +name 'TreeStructure' ,ParentID, [level] FROM x ";
        //        //query += " where FolderID in (select FolderID from tblFolder where DELFG = 0 ";
        //        //query += " ) ";
        //        //query += " ORDER BY [level] ";
        //        //query += " OPTION (MAXRECURSION 32) ";
        //        query = " WITH foldercte(FolderId, [Name], ParentID, LEVEL, treepath,treepath1) AS ";
        //        query += " ( SELECT FolderID AS FolderId, [Name], ParentID, 1 AS LEVEL, ";
        //        query += " CAST([Name] AS VARCHAR(8000)) AS treepath,CAST([Name] AS VARCHAR(8000)) AS treepath1 ";
        //        query += " FROM [tblFolder] WHERE ParentID = -1 ";
        //        query += " UNION ALL ";
        //        query += " SELECT d.FolderID AS FolderId, d.[Name], d.ParentID, ";
        //        query += " foldercte.LEVEL + 1 AS LEVEL, ";
        //        query += " CAST(foldercte.treepath + ' -> ' +CAST(d.[Name] AS VARCHAR(1024)) AS VARCHAR(8000)) AS treepath, ";
        //        query += " CAST(SPACE(5 * foldercte.LEVEL + 1)  +CAST(d.[Name] AS VARCHAR(1024)) AS VARCHAR(8000)) AS treepath1 ";
        //        query += " FROM [tblFolder] d INNER JOIN foldercte ON foldercte.FolderId = d.ParentID) ";
        //        query += " SELECT * FROM foldercte where FolderId in (select FolderID from [tblFolder] where DELFG = 0) ORDER BY treepath";

        //        SqlCommand cmd = new SqlCommand(query, con);
        //        cmd.CommandType = CommandType.Text;
        //        con.Open();
        //        SqlDataReader rdr = cmd.ExecuteReader();

        //        while (rdr.Read())
        //        {
        //            list.Add(new SelectListItem { Text = rdr["treepath1"].ToString().Replace(' ', Convert.ToChar(160)), Value = Convert.ToInt32(rdr["FolderId"]).ToString() });

        //        }
        //    }

        //    // list.Add(new SelectListItem { Text = "Instructor", Value = "Instructor" });
        //    //list.Add(new SelectListItem { Text = "Student", Value = "Student" });


        //    return list;


        //}

        
        
        


      
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
        //[Authorize(Roles = "A")]
        // GET: VideoDemo/Edit/5
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
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (Session["ApprovalFlag"].ToString() == "1")
                {
                    tblVideoDemo = db.tblMedias.Where(x => x.PhysicalDELFG == false && x.MediaID == id).SingleOrDefault();
                }
                else
                {
                    //tblVideoDemo = db.tblMedias.Find(id)
                    //
                    tblVideoDemo = db.tblMedias.Where(p => p.MediaID == id && p.PhysicalDELFG == false && p.CRTCD == uName && p.DELFG == false).SingleOrDefault();
                    //videoList = videoList.OrderByDescending(x => x.MediaID).ToList();
                    //.OrderByDescending(x => x.MediaID).ToList();
                }

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

        // POST: VideoDemo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [NonAction]
        //[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VideoId,Title,Description,Video,Thumbnail,IpAddress,DelFlg,CrDate,UdDate")] tblMedia tblVideoDemo)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;

            try
            {

                if (ModelState.IsValid)
                {
                    FillApprovalStatus();
                    List<SelectListItem> lstFolderName = new List<SelectListItem>();
                    lstFolderName = BLL.CommonLogic.FillFolderList();
                    lstFolderName.Insert(0, (new SelectListItem { Text = "Select", Value = "0" }));

                    // ViewData["FolderDtl"] = FillList();
                    ViewData["FolderDtl"] = lstFolderName;
                   

                    if (tblVideoDemo.PhysicalDELFG == true)
                    {
                        String strRootDirectory = Server.MapPath("~/");
                        string targetPath = strRootDirectory + @"\Video\";
                        if (tblVideoDemo.Video != null && tblVideoDemo.Video.Trim() != "" && tblVideoDemo.Video.Trim().Length > 4)
                        {
                            System.IO.FileInfo fiV = new System.IO.FileInfo(targetPath + tblVideoDemo.Video);
                            if (fiV.Exists)
                            {
                                fiV.Delete();
                            }
                        }

                        if (tblVideoDemo.Thumbnail != null && tblVideoDemo.Thumbnail.Trim() != "" && tblVideoDemo.Thumbnail.Trim().Length > 4)
                        {
                            System.IO.FileInfo fiV = new System.IO.FileInfo(targetPath + tblVideoDemo.Thumbnail);
                            if (fiV.Exists)
                            {
                                fiV.Delete();
                            }
                        }

                        //System.IO.FileInfo fiI = new System.IO.FileInfo(targetPath + tblVideoDemo.Thumbnail);
                    }

                    db.Entry(tblVideoDemo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
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

        // GET: VideoDemo/Delete/5
        [NonAction]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblMedia tblVideoDemo = db.tblMedias.Find(id);
            if (tblVideoDemo == null)
            {
                return HttpNotFound();
            }
            return View(tblVideoDemo);
        }

        // POST: VideoDemo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NonAction]
        public ActionResult DeleteConfirmed(int id)
        {
            tblMedia tblVideoDemo = db.tblMedias.Find(id);
            db.tblMedias.Remove(tblVideoDemo);
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
