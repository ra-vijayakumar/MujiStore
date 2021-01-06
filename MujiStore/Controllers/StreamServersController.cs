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
using System.Data;
using System.Data.SqlClient;
namespace MujiStore.Controllers
{
    [SessionExpire]
    [Authorize]
    [MUJICustomAuthorize(Roles = "16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31")]
    public class StreamServersController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();
        string CS = ConfigurationManager.ConnectionStrings["cnstr"].ConnectionString;
        // GET: StreamServers

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
                var selectList = new List<tblStreamServer>();
               
                string querySDtnt = "";

                using (SqlConnection con = new SqlConnection(CS))
                {
                    querySDtnt = " SELECT SRS.StreamServerID,  ";
                    querySDtnt += "convert(varchar,SN.SubnetID) + ' - ' + ST.StoreName + ': '+ SN.SNIPAddress +' / '+ SN.SubnetMask SubNetName, ";
                    querySDtnt += " DS.Name DeployScheduleName, SSServer, SRS.IPAddress,DriveCTotal,DriveCFree,DriveDTotal,DriveDFree,SRS.DELFG";
                    querySDtnt += " FROM tblStreamServer SRS ";
                    querySDtnt += " Left Join tblSubnet AS SN ON SN.SubnetID = SRS.BelongingSubnet ";
                    querySDtnt += " LEFT JOIN tblStoreSubnet AS SS ON SN.SubnetID = SS.Subnet  ";
                    querySDtnt += " LEFT JOIN tblStore AS ST ON SS.Store = ST.StoreID ";
                    querySDtnt += " LEFT Join tblDeploySchedule AS DS ON DS.DeployScheduleID = SRS.DeploySchedule ";
                    querySDtnt += " where SS.DELFG = 0 ORDER BY SRS.StreamServerID Desc ";
                    SqlCommand cmds = new SqlCommand(querySDtnt, con);
                    cmds.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader rdrs = cmds.ExecuteReader();
                    while (rdrs.Read())
                    {
                        
                            selectList.Add(new tblStreamServer
                            {
                                StreamServerID =Convert.ToInt32(rdrs["StreamServerID"]),
                                SubNetName = rdrs["SubNetName"].ToString(),
                                DeployScheduleName = rdrs["DeployScheduleName"].ToString(),
                                SSServer = rdrs["SSServer"].ToString(),
                                IPAddress = rdrs["IPAddress"].ToString(),
                                DVCTotal = rdrs["DriveCTotal"].ToString(),
                                DVCFree = rdrs["DriveCFree"].ToString(),
                                DVDTotal = rdrs["DriveDTotal"].ToString(),
                                DVDFree = rdrs["DriveDFree"].ToString(),
                                DELFG = Convert.ToBoolean(rdrs["DELFG"])
                            });

                    
                    }
                }
                //var tblStreamServers = db.tblStreamServers.Include(t => t.tblDeploySchedule).Include(t => t.tblSubnet).ToList().OrderByDescending(x => x.StreamServerID).ToPagedList(pageNumber, pageSize);
                return View(selectList.ToList().ToPagedList(pageNumber, pageSize));

                //return View(db.tblStoreGroupFolders.ToList().OrderByDescending(x => x.StoreGroupFolderID).ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: StreamServers/Details/5
        [NonAction]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStreamServer tblStreamServer = db.tblStreamServers.Find(id);
            if (tblStreamServer == null)
            {
                return HttpNotFound();
            }
            return View(tblStreamServer);
        }
        public void BindSubnet(int? val)
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select"
            });
            string querySDtnt = "";
   
            using (SqlConnection con = new SqlConnection(CS))
            {
                querySDtnt = "SELECT SN.SubnetID,  ";
                querySDtnt += "convert(varchar,SN.SubnetID) + ' - ' + ST.StoreName + ': '+ SN.SNIPAddress +' / '+ SN.SubnetMask SNetName ";
                querySDtnt += " FROM tblSubnet AS SN ";
                querySDtnt += " LEFT JOIN tblStoreSubnet AS SS ON SN.SubnetID = SS.Subnet ";
                querySDtnt += " LEFT JOIN tblStore AS ST ON SS.Store = ST.StoreID  ";
                querySDtnt += " where SN.DELFG = 0 and ss.DELFG = 0 and St.DELFG = 0 ";
                querySDtnt += " ORDER BY SN.SubnetID ASC ";
                SqlCommand cmds = new SqlCommand(querySDtnt, con);
                cmds.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdrs = cmds.ExecuteReader();
                while (rdrs.Read())
                {
                    if (val == Convert.ToInt32(rdrs["SubnetID"]))
                    {
                        selectList.Add(new SelectListItem
                        {
                            Value = rdrs["SubnetID"].ToString(),
                            Text = rdrs["SNetName"].ToString(),
                            Selected = true

                        });
                    }
                    else
                    {
                        selectList.Add(new SelectListItem
                        {
                            Value = rdrs["SubnetID"].ToString(),
                            Text = rdrs["SNetName"].ToString(),
                        });

                    }
                   
                }
            }
            ViewBag.BelongingSubnet = selectList;
         
        }
        public void BindDeploySchedule(int? val)
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select"
            });

            List<SelectListItem> items = new SelectList(db.tblDeploySchedules.Where(x => x.DELFG == false), "DeployScheduleID", "Name").ToList();
           
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

            ViewBag.DeploySchedule = selectList;
        }
        // GET: StreamServers/Create
        public ActionResult Create()
        {
           
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {

                BindSubnet(0);
                BindDeploySchedule(0);
                BindFormatList(0);
                //ViewBag.DeploySchedule = new SelectList(db.tblDeploySchedules.Where(x => x.DELFG == false), "DeployScheduleID", "Name");
                //ViewBag.BelongingSubnet = new SelectList(db.tblSubnets, "SubnetID", "SNIPAddress");
                return View();

                //ViewBag.FolderID = new SelectList(db.tblFolders.Where(x => x.DELFG == false), "FolderID", "Name");
                //ViewBag.StoreGroupID = new SelectList(db.tblStoreGroups.Where(x => x.DELFG == false), "StoreGroupID", "Name");
                //return View();
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: StreamServers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StreamServerID,FormatID,SSServer,IPAddress,BelongingSubnet,DeploySchedule,Status,LastDeployDate,LastStatusCheckDateDatetime,DriveCTotal,DriveCFree,DriveDTotal,DriveDFree,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,UserIPAddress")] tblStreamServer tblStreamServer)
        {
           
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;

            try
            {
                BindFormatList(tblStreamServer.FormatID);
                BindSubnet(tblStreamServer.BelongingSubnet);
                BindDeploySchedule(tblStreamServer.DeploySchedule);
                if (tblStreamServer.BelongingSubnet == 0)
                {
                    // ModelState["Store"] = true;
                    ModelState.AddModelError("BelongingSubnet", "Select Subnet");

                }
                if (tblStreamServer.DeploySchedule == 0)
                {
                    ModelState.AddModelError("DeploySchedule", "Select Deploy Schedule");
                }

                if(tblStreamServer.DriveCTotal != null && tblStreamServer.DriveCFree != null )
                {
                    if(tblStreamServer.DriveCTotal < tblStreamServer.DriveCFree)
                    {
                        ModelState.AddModelError("DriveCTotal", "Drive C Total space should less then Drive C Free");
                    }
                }
                if (tblStreamServer.DriveDTotal != null && tblStreamServer.DriveDFree != null)
                {
                    if (tblStreamServer.DriveDTotal < tblStreamServer.DriveDFree)
                    {
                        ModelState.AddModelError("DriveDTotal", "Drive D Total space should less then Drive C Free");
                    }
                }
                var tblssserver = db.tblStreamServers.Where(x => x.SSServer == tblStreamServer.SSServer && x.DELFG == false).FirstOrDefault();
                if (tblssserver != null)
                {
                    ModelState.AddModelError("SSServer", "Stream Server Already Exists");
                }
                var tblIPAdd = db.tblStreamServers.Where(x => x.IPAddress == tblStreamServer.IPAddress && x.DELFG == false).FirstOrDefault();
                if (tblIPAdd != null)
                {
                    ModelState.AddModelError("IPAddress", "IP Adress Already Exists");
                }

                var tblsubnt = db.tblStreamServers.Where(x => x.BelongingSubnet == tblStreamServer.BelongingSubnet && x.DELFG == false).FirstOrDefault();
                if (tblsubnt != null)
                {
                    ModelState.AddModelError("BelongingSubnet", "Subnet Already Exists");
                }
                if (ModelState.IsValid)
                {
                    LogInfo.Comments = "Stream Server Created - " + tblStreamServer.StreamServerID + "," + tblStreamServer.IPAddress;

                    tblStreamServer.CRTDT = DateTime.Now;
                    tblStreamServer.CRTCD = Session["UserName"].ToString();
                    tblStreamServer.UserIPAddress = Session["IPAddress"].ToString();
                   // tblStreamServer.DELFG = false;
                    //tblVideoDemoStoreMst.UdDate = DateTime.Now;
                    db.tblStreamServers.Add(tblStreamServer);
                    db.SaveChanges();

                    tblStreamServerFormat ssf = new tblStreamServerFormat();
                    ssf.SSFServer = tblStreamServer.SSServer;
                    ssf.FormatID = tblStreamServer.FormatID;
                    ssf.DELFG = tblStreamServer.DELFG;
                    ssf.CRTCD = Session["UserName"].ToString();
                    ssf.CRTDT = DateTime.Now;
                    ssf.IPAddress = Session["IPAddress"].ToString();
                    db.tblStreamServerFormats.Add(ssf);
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);
                    TempData["SuccMsg"] = MujiStore.Resources.Resource.CntStoreGroupFoldersCreateSuccMsg;
                    return RedirectToAction("Index");
                }
                if (ModelState.IsValid)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonCreateUnableCreateStreamSer;
                }
                return View(tblStreamServer);
            }
            catch (Exception ex)
            {

                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // GET: StreamServers/Edit/5
        public ActionResult Edit(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //tblStreamServer tblStreamServer = db.tblStreamServers.Find(id);
            //if (tblStreamServer == null)
            //{
            //    return HttpNotFound();
            //}
            //ViewBag.DeploySchedule = new SelectList(db.tblDeploySchedules, "DeployScheduleID", "Name", tblStreamServer.DeploySchedule);
            //ViewBag.BelongingSubnet = new SelectList(db.tblSubnets, "SubnetID", "SNIPAddress", tblStreamServer.BelongingSubnet);
            //return View(tblStreamServer);
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblStreamServer tblStreamServer = db.tblStreamServers.Find(id);
                if (tblStreamServer == null)
                {
                    return HttpNotFound();
                }
                var tblStreamServerfmt = db.tblStreamServerFormats.Where(x => x.SSFServer == tblStreamServer.SSServer).ToList();
                int fmtid = 0;
                foreach(tblStreamServerFormat ssf in tblStreamServerfmt)
                {
                   
                    fmtid = ssf.FormatID ?? default(int);
                }
                tblStreamServer.FormatID = fmtid;
                BindSubnet(tblStreamServer.BelongingSubnet);
                BindDeploySchedule(tblStreamServer.DeploySchedule);
                BindFormatList(fmtid);
                tblStreamServer.OrigSSServer = tblStreamServer.SSServer;
        
                //ViewBag.DeploySchedule = new SelectList(db.tblDeploySchedules, "DeployScheduleID", "Name", tblStreamServer.DeploySchedule);
                //ViewBag.BelongingSubnet = new SelectList(db.tblSubnets, "SubnetID", "SNIPAddress", tblStreamServer.BelongingSubnet);
                return View(tblStreamServer);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }

        // POST: StreamServers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StreamServerID,OrigSSServer,SSServer,FormatID,IPAddress,BelongingSubnet,DeploySchedule,Status,LastDeployDate,LastStatusCheckDateDatetime,DriveCTotal,DriveCFree,DriveDTotal,DriveDFree,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,UserIPAddress")] tblStreamServer tblStreamServer)
        {
           
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {


                BindSubnet(tblStreamServer.BelongingSubnet);
                BindDeploySchedule(tblStreamServer.DeploySchedule);
                BindFormatList(tblStreamServer.FormatID);
                if (tblStreamServer.BelongingSubnet == 0)
                {
                    // ModelState["Store"] = true;
                    ModelState.AddModelError("BelongingSubnet", "Select Subnet");

                }
                if (tblStreamServer.DeploySchedule == 0)
                {
                    ModelState.AddModelError("DeploySchedule", "Select Deploy Schedule");
                }

                if (tblStreamServer.DriveCTotal != null && tblStreamServer.DriveCFree != null)
                {
                    if (tblStreamServer.DriveCTotal < tblStreamServer.DriveCFree)
                    {
                        ModelState.AddModelError("DriveCTotal", "Drive C Total space should less then Drive C Free");
                    }
                }
                if (tblStreamServer.DriveDTotal != null && tblStreamServer.DriveDFree != null)
                {
                    if (tblStreamServer.DriveDTotal < tblStreamServer.DriveDFree)
                    {
                        ModelState.AddModelError("DriveDTotal", "Drive D Total space should less then Drive C Free");
                    }
                }
                var tblssserver = db.tblStreamServers.Where(x => x.SSServer == tblStreamServer.SSServer && x.StreamServerID != tblStreamServer.StreamServerID  && x.DELFG == false).FirstOrDefault();
                if (tblssserver != null)
                {
                    ModelState.AddModelError("SSServer", "Stream Server Already Exists");
                }
                var tblIPAdd = db.tblStreamServers.Where(x => x.IPAddress == tblStreamServer.IPAddress && x.StreamServerID != tblStreamServer.StreamServerID  && x.DELFG == false).FirstOrDefault();
                if (tblIPAdd != null)
                {
                    ModelState.AddModelError("IPAddress", "IP Adress Already Exists");
                }

                var tblsubnt = db.tblStreamServers.Where(x => x.BelongingSubnet == tblStreamServer.BelongingSubnet && x.StreamServerID != tblStreamServer.StreamServerID  && x.DELFG == false).FirstOrDefault();
                if (tblsubnt != null)
                {
                    ModelState.AddModelError("BelongingSubnet", "Subnet Already Exists");
                }


                if (ModelState.IsValid)
                {
                    int result = 0;

                    LogInfo.Comments = "Stream Server Updated - " + tblStreamServer.SSServer.ToString() + "," + tblStreamServer.IPAddress.ToString();
                    tblStreamServer.UPDDT = DateTime.Now;
                    tblStreamServer.UPDCD = Session["UserName"].ToString();
                    tblStreamServer.UserIPAddress = Session["IPAddress"].ToString();
                    db.Entry(tblStreamServer).State = EntityState.Modified;
                    db.SaveChanges();
                    CommonLogic.Log_info(LogInfo.MenuClick, LogInfo.Comments);

                    /* Update Stream Server format */

                    using (SqlConnection con = new SqlConnection(CS))
                    {
                        con.Open();
                        string query = "";

                        query = "if exists (select * from tblStreamServerFormat where  SSFServer = @OrigSSServer) ";
                        query += " begin ";
                        query += " update tblStreamServerFormat set SSFServer = @SSFServer,FormatID=@FormatID,DELFG=@DELFG,UPDDT=@UPDDT,UPDCD=@UPDCD,IPAddress=@IPAddress where SSFServer = @OrigSSServer;";
                        query += " end ";
                        query += " else ";
                        query += " begin ";
                        query += " insert into tblStreamServerFormat(SSFServer,FormatID,DELFG,CRTDT,CRTCD,IPAddress) values ";
                        query += " (@SSFServer, @FormatID, @DELFG, @UPDDT, @UPDCD, @IPAddress) ";
                        query += " end";

                        SqlCommand cmd;
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@OrigSSServer", tblStreamServer.OrigSSServer);
                        cmd.Parameters.AddWithValue("@SSFServer", tblStreamServer.SSServer);
                        cmd.Parameters.AddWithValue("@FormatID", tblStreamServer.FormatID);
                        cmd.Parameters.AddWithValue("@DELFG", tblStreamServer.DELFG);
                        cmd.Parameters.AddWithValue("@UPDDT", tblStreamServer.UPDDT);
                        cmd.Parameters.AddWithValue("@UPDCD", tblStreamServer.UPDCD);
                        cmd.Parameters.AddWithValue("@IPAddress", tblStreamServer.IPAddress);
                    
                        cmd.CommandType = CommandType.Text;

                        result = cmd.ExecuteNonQuery();
                    }
                        TempData["SuccMsg"] = "Stream Server Updated Successfully";
                    return RedirectToAction("Index");
                }
         
                if (ModelState.IsValid)
                {
                    TempData["ErrMsg"] = MujiStore.Resources.Resource.CommonCreateUnableUpdateStreamSer;
                }
                return View(tblStreamServer);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                TempData["ErrMsg"] = "Unable Update Stream Server";
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
        }
        public void BindFormatList(int? val)
        {

            string query = "";
            var list = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                //SqlCommand cmd = new SqlCommand("Select VideoId,Title,Description,Video,Thumbnail,FoderID from [tblVideoDemo]", con);
                query = "Select FormatID,Name from tblFormat where Delfg = 0 Order by Name ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                list.Add(new SelectListItem { Text = "Select", Value = "0" });
                while (rdr.Read())
                {
                    if(Convert.ToInt32(rdr["FormatID"]).ToString() == Convert.ToString(val))
                    {
                        list.Add(new SelectListItem { Text = rdr["Name"].ToString(), Value = Convert.ToInt32(rdr["FormatID"]).ToString(), Selected = true });
                    }
                    else
                    {
                        list.Add(new SelectListItem { Text = rdr["Name"].ToString(), Value = Convert.ToInt32(rdr["FormatID"]).ToString() });
                    }
                    

                }
            }

            // list.Add(new SelectListItem { Text = "Instructor", Value = "Instructor" });
            //list.Add(new SelectListItem { Text = "Student", Value = "Student" });

            ViewBag.FormatID = list;
         
        }

        // GET: StreamServers/Delete/5
        [NonAction]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStreamServer tblStreamServer = db.tblStreamServers.Find(id);
            if (tblStreamServer == null)
            {
                return HttpNotFound();
            }
            return View(tblStreamServer);
        }

        // POST: StreamServers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NonAction]
        public ActionResult DeleteConfirmed(int id)
        {
            tblStreamServer tblStreamServer = db.tblStreamServers.Find(id);
            db.tblStreamServers.Remove(tblStreamServer);
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
