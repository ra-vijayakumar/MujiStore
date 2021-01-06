using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc;
using MujiStore.Models;
using System.Configuration;
using System.Data.SqlClient;
using MujiStore.BLL;
using System.Diagnostics;
using PagedList;
using System.Text;
using System.Threading;
using System.Data;
using System.Net;

namespace MujiStore.Controllers
{
    [SessionExpire]
    [Authorize]
    // [Authorize(Roles = "A,U")]
    [MUJICustomAuthorize(Roles = "8,9,10,11,12,13,14,15,24,25,27,26,28,29,30,31")]
    public class MediaApprovalController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();
        string CS = ConfigurationManager.ConnectionStrings["cnstr"].ConnectionString;
        // GET: MediaApproval
        public ActionResult Index(string sortOrder, int? page, string SearTitle, string SearchTitle, string SearchFromCRTDT, string CdStDate, string SearchToCRTDT, string CdEdDate, int? ConvertStatus, int? CvtStatus, int? DelFlag, int? DFlag, int? FolderID, int? FolID,int? MediaID, int? MedID )
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            int pageSize;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ApplicationIDSortParm = sortOrder == "ApplicationID" ? "ApplicationID_desc" : "ApplicationID";
            ViewBag.MediaIDSortParm = sortOrder == "MediaID" ? "MediaID_desc" : "MediaID";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.ConvertStatusSortParm = sortOrder == "ConvertStatus" ? "ConvertStatus_desc" : "ConvertStatus";
            ViewBag.FolderSortParm = sortOrder == "Folder" ? "Folder_desc" : "Folder";

            List<SelectListItem> lstConvertStatus = new List<SelectListItem>();

            lstConvertStatus.Add(new SelectListItem { Text = "All", Value = "4" });
            lstConvertStatus.Add(new SelectListItem { Text = "Approved", Value = "3" });//VJ 20200603 ConvertStatus Changed
            lstConvertStatus.Add(new SelectListItem { Text = "Un Approved", Value = "0" });
            ViewBag.ConvertStatus = new SelectList(lstConvertStatus, "Value", "Text", CvtStatus);



            List<tblMedia> VidList = new List<tblMedia>();
            List<SelectListItem> lstFolderName = new List<SelectListItem>();
            List<SelectListItem> lstFolderNameBulk = new List<SelectListItem>();
            List<SelectListItem> lstMediaDtl = new List<SelectListItem>();
            lstFolderName =BLL.CommonLogic.FillFolderList();
            lstFolderNameBulk = BLL.CommonLogic.FillFolderList();
            ViewBag.FolderList = new SelectList(lstFolderNameBulk, "Value", "Text");
            lstFolderName.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.FolderID = new SelectList(lstFolderName, "Value", "Text", FolID);
            lstMediaDtl = BindMediaDetails();
            ViewBag.MediaID = new SelectList(lstMediaDtl, "Value", "Text", MedID);

            if (SearchTitle != null || SearchFromCRTDT != null || SearchToCRTDT != null || ConvertStatus != null || DelFlag != null)
            {
                page = 1;
            }
            else
            {
                SearchTitle = SearTitle;
                SearchFromCRTDT = CdStDate;
                SearchToCRTDT = CdEdDate;
                ConvertStatus = CvtStatus;
                MediaID = MedID;
                FolderID = FolID;
            }
            ViewBag.SearTitle = SearchTitle;
            ViewBag.CdStDate = SearchFromCRTDT;
            ViewBag.CdEdDate = SearchToCRTDT;
            ViewBag.cvtStatus = ConvertStatus;
            ViewBag.MedID = MediaID;
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
                string query = " Select ApplicationID,MA.MediaID,MA.Title,MA.Description,Name,NewApprovalStatus,[Delete] IsDelete, ";
                query += " MA.Registerer,MA.Accepter,Memo,RegisteredDate,CompleteDate,Approved,Video,Thumbnail ";
                query += " from tblApplication MA ";
                query += " left join tblFolder F on F.FolderID = MA.FolderID ";
                query += " Left Join tblMedia  M on M.MediaID = MA.MediaID ";
                query += " where M.PhysicalDELFG = 0 and MA.DelFG = 0  and MA.CompleteDate is null  ";
                query += " and FORMAT(MA.RegisteredDate, 'yyyy-MM-dd') between '" + CdStDate + "' and '" + CdEdDate + "'";
                //query = query + " lastName like '%" + Name + "%' and EnrollmentDate ";
                
                if(ViewBag.MedID != null && ViewBag.MedID != 0)
                {
                    query = query + " and M.MediaID = " + ViewBag.MedID;
                }

                if (ViewBag.SearTitle != null && ViewBag.SearTitle != "" && ViewBag.SearTitle != string.Empty)
                {
                    query = query + " and MA.title like '%" + ViewBag.SearTitle + "%'";
                }

                if (ViewBag.cvtStatus != null && ViewBag.cvtStatus != 4)
                {
                    query = query + " and MA.NewApprovalStatus = " + ViewBag.cvtStatus;
                }
                if (ViewBag.FolID != null && ViewBag.FolID != 0)
                {
                    query = query + " and MA.FolderID = " + Convert.ToInt32(ViewBag.FolID);
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
                                    ApplicationID = Convert.ToInt32(sdr["ApplicationID"]),
                                    MediaID = Convert.ToInt32(sdr["MediaID"]),
                                    Title = Convert.ToString(sdr["Title"]),
                                    Description = Convert.ToString(sdr["Description"]),
                                    FolderName = Convert.ToString(sdr["Name"]),
                                    ApprovalStatus = Convert.ToInt32(sdr["NewApprovalStatus"]),
                                    PhysicalDELFG = Convert.ToBoolean(sdr["IsDelete"]),
                                    Registerer = Convert.ToString(sdr["Registerer"]),
                                    CRTDT = Convert.ToDateTime(sdr["RegisteredDate"]),
                                    Video = Convert.ToString(sdr["Video"]),
                                    Thumbnail = Convert.ToString(sdr["Thumbnail"]),
                
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
                    case "ApplicationID":
                        videoList = videoList.OrderBy(o => o.ApplicationID).ToList();
                        break;
                    case "MediaID":
                        videoList = videoList.OrderBy(o => o.MediaID).ToList();
                        break;
                    case "MediaID_Desc":
                        videoList = videoList.OrderByDescending(o => o.MediaID).ToList();
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
                        videoList = videoList.OrderByDescending(o => o.ApplicationID).ToList();
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

        public tblMedia GetApplicationDetails(int? id)
        {
            tblMedia tblApp = new tblMedia();

            string query = " Select ApplicationID,MA.MediaID,MA.Title,MA.Description,Name,NewApprovalStatus,[Delete] IsDelete, ";
            query += " MA.Registerer,MA.Accepter,Memo,RegisteredDate,CompleteDate,Approved,Video,Thumbnail,Memo,MA.FolderID ";
            query += " from tblApplication MA ";
            query += " left join tblFolder F on F.FolderID = MA.FolderID ";
            query += " Left Join tblMedia  M on M.MediaID = MA.MediaID ";
            query += " where M.PhysicalDELFG = 0 and MA.DelFG = 0  and MA.CompleteDate is null  ";
            query += " and MA.ApplicationID =" + id;

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

                            // MediaID,Title,Description,Video,ConvertStatus,FolderID
                            tblApp.ApplicationID = Convert.ToInt32(sdr["ApplicationID"]);
                            tblApp.MediaID = Convert.ToInt32(sdr["MediaID"]);
                            tblApp.Title = Convert.ToString(sdr["Title"]);
                            tblApp.Description = Convert.ToString(sdr["Description"]);
                            tblApp.FolderName = Convert.ToString(sdr["Name"]);
                            tblApp.FolderID = Convert.ToInt32(sdr["FolderID"]);
                            tblApp.ApprovalStatus = Convert.ToInt32(sdr["NewApprovalStatus"]);
                            tblApp.PhysicalDELFG = Convert.ToBoolean(sdr["IsDelete"]);
                            tblApp.Comments = Convert.ToString(sdr["Memo"]);
                            tblApp.Registerer = Convert.ToString(sdr["Registerer"]);
                            tblApp.CRTDT = Convert.ToDateTime(sdr["RegisteredDate"]);
                            tblApp.Video = Convert.ToString(sdr["Video"]);
                            tblApp.Thumbnail = Convert.ToString(sdr["Thumbnail"]);


                        }
                    }
                    con.Close();
                }
            }
            return tblApp;
        }
        public ActionResult Edit(int? id)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            tblMedia tblApp = new tblMedia();
            try
            {
                string uName = Session["UserName"].ToString();


                

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }


                tblApp = GetApplicationDetails(id);


                //tblMedia tblVideoDemo = db.tblMedias.Find(id);
                if (tblApp.ApplicationID == 0)
                {
                    return HttpNotFound();
                }
               


                return View(tblApp);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ApplicationID,MediaID,NewApprovalStatus")] tblMedia tblVideoDemo)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            tblMedia med = new tblMedia();
            string query = "";
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd;
                    if (tblVideoDemo.NewApprovalStatus == 1)
                    {
                        med = GetApplicationDetails(tblVideoDemo.ApplicationID);
                        if (med.PhysicalDELFG == false)
                        {
                            query = "Update tblMedia set Title=@Title,Description=@Description,";
                            query += "Accepter=@Accepter,FolderID=@FolderID,ApprovalStatus=@ApprovalStatus,";
                            query += "IpAddress=@IpAddress,UPDDT=@UPDDT,UPDCD=@UPDCD";
                            query += " Where MediaID=@MediaID";

                            cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@Title", med.Title);
                            cmd.Parameters.AddWithValue("@Description", med.Description);
                            cmd.Parameters.AddWithValue("@FolderID", med.FolderID);
                            cmd.Parameters.AddWithValue("@Accepter", Session["UserName"].ToString());
                            cmd.Parameters.AddWithValue("@ApprovalStatus", med.ApprovalStatus);
                            cmd.Parameters.AddWithValue("@UPDCD", Session["UserName"].ToString());
                            cmd.Parameters.AddWithValue("@UPDDT", DateTime.Now);
                            cmd.Parameters.AddWithValue("@IpAddress", Session["IPAddress"].ToString());
                            cmd.Parameters.AddWithValue("@MediaID", med.MediaID);
                            cmd.CommandType = CommandType.Text;
                            result = cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();
                            query = "";
                            query += "Update tblApplication set Accepter = @Accepter,CompleteDate = GETDATE(),";
                            query += "Approved =1,IpAddress=@IpAddress,UPDDT=@UPDDT,UPDCD=@UPDCD ";
                            query += " Where ApplicationID=@ApplicationID";

                            cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@Accepter", Session["UserName"].ToString());
                            cmd.Parameters.AddWithValue("@UPDCD", Session["UserName"].ToString());
                            cmd.Parameters.AddWithValue("@UPDDT", DateTime.Now);
                            cmd.Parameters.AddWithValue("@IpAddress", Session["IPAddress"].ToString());
                            cmd.Parameters.AddWithValue("@ApplicationID", med.ApplicationID);
                            cmd.CommandType = CommandType.Text;
                            result = cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();
                           // return RedirectToAction("Index");
                        }
                        else
                        {

                            // Physical Deletion commented VJ 20200605
                            var fileName = "";
                            String strRootDirectory = Server.MapPath("~/");
                                string targetPath = strRootDirectory + @"\Video\";
                                if (med.Video != null && med.Video.Trim() != "" && med.Video.Trim().Length > 4)
                                {
                                    System.IO.FileInfo fiV = new System.IO.FileInfo(targetPath + med.Video);
                                    if (fiV.Exists)
                                    {
                                        fiV.Delete();
                                    }
                                }
                                fileName = med.Video.ToLower().Replace(".mp4", ".png");
                                if (fileName != null && fileName.Trim() != "" && fileName.Trim().Length > 4)
                                {
                                    System.IO.FileInfo fiV = new System.IO.FileInfo(targetPath + fileName);
                                    if (fiV.Exists)
                                    {
                                        fiV.Delete();
                                    }
                                }
                    //Physical Deletion commented VJ 20200605 */

                            query = "";
                            query += " Update tblMedia set PhysicalDELFG=@PhysicalDELFG,PhysicalDELFGCRTCD=@PhysicalDELFGCRTCD, ";
                            query += " PhysicalDELFGCRTDT=@PhysicalDELFGCRTDT, ";
                            query += " PhysicalDELIpAddress=@PhysicalDELIpAddress,UPDDT=@UPDDT,UPDCD=@UPDCD,IpAddress=@IpAddress ";
                            query += " Where MediaID=@MediaID";

                            cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@PhysicalDELFG", med.PhysicalDELFG);
                            cmd.Parameters.AddWithValue("@PhysicalDELFGCRTCD", Session["UserName"].ToString());
                            cmd.Parameters.AddWithValue("@PhysicalDELFGCRTDT", DateTime.Now);
                            cmd.Parameters.AddWithValue("@PhysicalDELIpAddress", Session["IPAddress"].ToString());
                            cmd.Parameters.AddWithValue("@UPDCD", Session["UserName"].ToString());
                            cmd.Parameters.AddWithValue("@UPDDT", DateTime.Now);
                            cmd.Parameters.AddWithValue("@IpAddress", Session["IPAddress"].ToString());
                            cmd.Parameters.AddWithValue("@MediaID", med.MediaID);
                            cmd.CommandType = CommandType.Text;
                            result = cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();

                            query = "";
                            query = " Update tblMediaFormatInfo set ";
                            query += " DELFG=@DELFG,IpAddress=@IpAddress,UPDDT=@UPDDT,UPDCD=@UPDCD ";
                            query += " Where MediaID=@MediaID";
                            cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@DELFG", med.PhysicalDELFG);
                            cmd.Parameters.AddWithValue("@UPDDT", DateTime.Now);
                            cmd.Parameters.AddWithValue("@UPDCD", Session["UserName"].ToString());
                            cmd.Parameters.AddWithValue("@IpAddress", Session["IPAddress"].ToString());
                            cmd.Parameters.AddWithValue("@MediaID", med.MediaID);
                            cmd.CommandType = CommandType.Text;
                            result = cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();

                            query = "";
                            query += "Update tblApplication set Accepter = @Accepter,CompleteDate = GETDATE(),";
                            query += "Approved =1,IpAddress=@IpAddress,UPDDT=@UPDDT,UPDCD=@UPDCD ";
                            query += " Where ApplicationID=@ApplicationID";

                            cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@Accepter", Session["UserName"].ToString());
                            cmd.Parameters.AddWithValue("@UPDCD", Session["UserName"].ToString());
                            cmd.Parameters.AddWithValue("@UPDDT", DateTime.Now);
                            cmd.Parameters.AddWithValue("@IpAddress", Session["IPAddress"].ToString());
                            cmd.Parameters.AddWithValue("@ApplicationID", med.ApplicationID);
                            cmd.CommandType = CommandType.Text;
                            result = cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();
                            //return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        query = "";
                        query += "Update tblApplication set Accepter = @Accepter,CompleteDate = GETDATE(),";
                        query += "Approved =0,IpAddress=@IpAddress,UPDDT=@UPDDT,UPDCD=@UPDCD ";
                        query += " Where ApplicationID=@ApplicationID";

                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Accepter", Session["UserName"].ToString());
                        cmd.Parameters.AddWithValue("@UPDCD", Session["UserName"].ToString());
                        cmd.Parameters.AddWithValue("@UPDDT", DateTime.Now);
                        cmd.Parameters.AddWithValue("@IpAddress", Session["IPAddress"].ToString());
                        cmd.Parameters.AddWithValue("@ApplicationID", med.ApplicationID);
                        cmd.CommandType = CommandType.Text;
                        result = cmd.ExecuteNonQuery();

                        //return RedirectToAction("Index");
                    }
                    
                    
                }
                LogInfo.Comments = "Video Approval Updated - " + med.ApplicationID.ToString();
                CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                //db.SaveChanges();
                TempData["SuccMsg"] = MujiStore.Resources.Resource.CntVideoDemoSaveThumbnailImageSuccMsg;
                //do somthing with model
                return RedirectToAction("Index");

                //tblMedia tblApp = new tblMedia();
                //tblApp = GetApplicationDetails(tblVideoDemo.ApplicationID);
                //return View(tblApp);
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

        public List<SelectListItem> BindMediaDetails()
        {

            List<SelectListItem> items = new SelectList(db.tblMedias.Where(x => x.PhysicalDELFG == false), "MediaID", "MediaID").ToList();
            // items.Insert(0, (new SelectListItem { Text = "Select", Value = null }));

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select"
            });
            foreach (var element in items)
            {
                
                    selectList.Add(new SelectListItem
                    {
                        Value = element.Value,
                        Text = element.Text
                    });

       
            }

            return selectList;
           


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
    }
}