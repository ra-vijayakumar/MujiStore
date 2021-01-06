using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MujiStore.BLL;
using MujiStore.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace MujiStore.Controllers
{
    [SessionExpire]
    [Authorize]
    [MUJICustomAuthorize(Roles = "2,3,6,7,10,11,14,15,18,19,22,23,26,27,30,31")]
    public class DeployStatController : Controller
    {
        string CS = ConfigurationManager.ConnectionStrings["cnstr"].ConnectionString;
        // GET: DeployStat
        public ActionResult Index()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                List<tblStreamServer> sslist = new List<tblStreamServer>();
                List<tblDeployStatu> deplist = new List<tblDeployStatu>();
                DataColumn column;
                DataTable SereamServerList = new DataTable("tblStreamServer");
                // DataColumn dtColumn;
                DataRow sserver;
                SereamServerList.Columns.Add("SSServer");
                SereamServerList.Columns.Add("StoreID", typeof(Int32));
                SereamServerList.Columns.Add("StoreName");
                SereamServerList.Columns.Add("StoreGroupName");

                SereamServerList.Columns.Add("DriveCTotal", typeof(long));
                //column = new DataColumn();
                //column.DataType = System.Type.GetType("System.Int32");
                //column.ColumnName = "DriveCTotal";
                //column.AllowDBNull = true;
                //SereamServerList.Columns.Add(column);
                SereamServerList.Columns.Add("DriveCFree", typeof(long));

                //column = new DataColumn();
                //column.DataType = System.Type.GetType("System.Int32");
                //column.ColumnName = "DriveCFree";
                //column.AllowDBNull = true;
                //SereamServerList.Columns.Add(column);

                SereamServerList.Columns.Add("DriveDTotal", typeof(long));
                //column = new DataColumn();
                //column.DataType = System.Type.GetType("System.Int32");
                //column.ColumnName = "DriveDTotal";
                //column.AllowDBNull = true;
                //SereamServerList.Columns.Add(column);

                SereamServerList.Columns.Add("DriveDFree", typeof(long));
                //column = new DataColumn();
                //column.DataType = System.Type.GetType("System.Int32");
                //column.ColumnName = "DriveDFree";
                //column.AllowDBNull = true;
                //SereamServerList.Columns.Add(column);

                SereamServerList.Columns.Add("NotExistsMediaCount", typeof(long));
                SereamServerList.Columns.Add("ExistsMediaCount", typeof(long));
                SereamServerList.Columns.Add("NotExistsFileSize", typeof(long));
                SereamServerList.Columns.Add("ExistsFileSize", typeof(long));




                string querySDtnt = "";
                string SSDetails = "";
                using (SqlConnection con = new SqlConnection(CS))
                {
                    querySDtnt = "SELECT SG.Name AS StoreGroupName, ST.StoreID AS StoreID, ST.StoreName AS StoreName,  ";
                    querySDtnt += " SSV.SSServer AS StreamServer, isnull(SSV.DriveCTotal,0) DriveCTotal, isnull(SSV.DriveCFree,0) DriveCFree,  ";
                    querySDtnt += " isnull(SSV.DriveDTotal,0) DriveDTotal, isnull(SSV.DriveDFree,0) DriveDFree, ";
                    querySDtnt += " 0 NotExistsMediaCount,0 ExistsMediaCount,0 NotExistsFileSize,0 ExistsFileSize ";
                    querySDtnt += " FROM tblStreamServer AS SSV ";
                    querySDtnt += " LEFT JOIN tblStoreSubnet AS SSN ON SSV.BelongingSubnet = SSN.Subnet ";
                    querySDtnt += " LEFT JOIN tblStore AS ST ON SSN.Store = ST.StoreID ";
                    querySDtnt += " LEFT JOIN tblStoreGroup AS SG ON ST.StoreGroupID = SG.StoreGroupID ";
                    querySDtnt += " where SSV.DELFG = 0 AND SSN.DELFG = 0 AND ST.DELFG = 0 AND SG.DELFG = 0 ";
                    querySDtnt += " ORDER BY ST.StoreID ASC ";

                    SqlCommand cmds = new SqlCommand(querySDtnt, con);
                    cmds.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader rdrs = cmds.ExecuteReader();
                    while (rdrs.Read())
                    {
                        sserver = SereamServerList.NewRow();
                        sserver["SSServer"] = rdrs["StreamServer"].ToString();
                        sserver["StoreID"] = Convert.ToInt32(rdrs["StoreID"]);
                        sserver["StoreName"] = rdrs["StoreName"].ToString();
                        sserver["StoreGroupName"] = rdrs["StoreGroupName"].ToString();
                        sserver["DriveCTotal"] = Convert.ToDouble(rdrs["DriveCTotal"]);
                        sserver["DriveCFree"] = Convert.ToDouble(rdrs["DriveCFree"]);
                        sserver["DriveDTotal"] = Convert.ToDouble(rdrs["DriveDTotal"]);
                        sserver["DriveDFree"] = Convert.ToDouble(rdrs["DriveDFree"]);
                        sserver["NotExistsMediaCount"] = Convert.ToDouble(rdrs["NotExistsMediaCount"]);
                        sserver["ExistsMediaCount"] = Convert.ToDouble(rdrs["ExistsMediaCount"]);
                        sserver["NotExistsFileSize"] = Convert.ToDouble(rdrs["NotExistsFileSize"]);
                        sserver["ExistsFileSize"] = Convert.ToDouble(rdrs["ExistsFileSize"]);
                        SereamServerList.Rows.Add(sserver);
                        sslist.Add(new tblStreamServer()
                        {
                            SSServer = rdrs["StreamServer"].ToString(),
                            StoreID = Convert.ToInt16(rdrs["StoreID"]),
                            StoreName = rdrs["StoreName"].ToString(),
                            StoreGroupName = rdrs["StoreGroupName"].ToString(),
                            DriveCTotal = Convert.ToInt64(rdrs["DriveCTotal"]),
                            DriveCFree = Convert.ToInt64(rdrs["DriveCFree"]),
                            DriveDTotal = Convert.ToInt64(rdrs["DriveDTotal"]),
                            DriveDFree = Convert.ToInt64(rdrs["DriveDFree"]),
                            NotExistsMediaCount = Convert.ToInt64(rdrs["NotExistsMediaCount"]),
                            ExistsMediaCount = Convert.ToInt64(rdrs["ExistsMediaCount"]),
                            NotExistsFileSize = Convert.ToInt64(rdrs["NotExistsFileSize"]),
                            ExistsFileSize = Convert.ToInt64(rdrs["ExistsFileSize"])
                        });

                        SSDetails += "'" + rdrs["StreamServer"].ToString() + "',";


                        //ste.StoreID = Convert.ToInt32(rdrs["StoreID"]);
                        //ste.StoreName = rdrs["StoreName"].ToString();
                        //stelist.Add(ste);
                    }

                    rdrs.Close();
                    if(SSDetails.Length > 0)
                    {
                        SSDetails = SSDetails.Remove(SSDetails.Length - 1);

                        querySDtnt = "";

                        querySDtnt = "SELECT SV.SSServer, DS.IsExists, isnull(COUNT(M.MediaID),0) AS MediaCount, ";
                        querySDtnt += " isnull(SUM(MFI.FileSize),0) AS TotalFileSize ";
                        querySDtnt += " FROM tblStore AS ST ";
                        querySDtnt += " LEFT JOIN tblStoreSubnet AS STSN ON ST.StoreID = STSN.Store ";
                        querySDtnt += " LEFT JOIN tblStreamServer AS SV ON STSN.Subnet = SV.BelongingSubnet ";
                        querySDtnt += " LEFT JOIN tblStoreGroupFolder AS SGFL ON ST.StoreGroupID = SGFL.StoreGroupID ";
                        querySDtnt += " RIGHT JOIN tblMedia AS M ON SGFL.FolderID = M.FolderID ";
                        querySDtnt += " LEFT JOIN tblStreamServerFormat AS SF ON SV.SSServer = SF.SSFServer ";
                        querySDtnt += " LEFT JOIN tblMediaFormatInfo AS MFI ON M.MediaID = MFI.MediaID AND SF.FormatID = MFI.FormatID ";
                        querySDtnt += " LEFT JOIN tblDeployStatus AS DS ON M.MediaID = DS.MediaID AND SF.FormatID = DS.FormatID ";
                        querySDtnt += " AND DS.DSServer = SV.SSServer ";
                        querySDtnt += " WHERE M.DELFG = 0 AND M.PhysicalDELFG = 0  AND SV.SSServer in (" + @SSDetails + ") ";
                        querySDtnt += " AND ST.DELFG = 0 AND STSN.DELFG = 0 AND SV.DELFG = 0 AND SGFL.DELFG = 0 ";
                        querySDtnt += " AND SF.DELFG = 0 AND MFI.DELFG = 0 AND DS.DELFG = 0 ";
                        querySDtnt += " GROUP BY SV.SSServer, DS.IsExists Order by SV.SSServer";

                        SqlCommand cmddss = new SqlCommand(querySDtnt, con);
                        cmddss.CommandType = CommandType.Text;
                        //cmddss.Parameters.AddWithValue("@SSDetails", SSDetails);
                        SqlDataReader rdrds = cmddss.ExecuteReader();
                        // List<tblDeployStatu> deplist = new List<tblDeployStatu>();
                        while (rdrds.Read())
                        {
                            deplist.Add(new tblDeployStatu()
                            {
                                DSServer = rdrds["SSServer"].ToString(),
                                FormatName = rdrds["IsExists"].ToString(),
                                MediaCount = Convert.ToInt64(rdrds["MediaCount"]),
                                TotalFileSize = Convert.ToInt64(rdrds["TotalFileSize"])

                            });

                            for (int i = 0; i < SereamServerList.Rows.Count; i++)
                            {
                                if (SereamServerList.Rows[i]["SSServer"].ToString() == rdrds["SSServer"].ToString())
                                {
                                    if (rdrds["IsExists"].ToString() == null || rdrds["IsExists"].ToString() == "" || rdrds["IsExists"].ToString() == string.Empty || rdrds["IsExists"].ToString() == "False")
                                    {
                                        SereamServerList.Rows[i]["NotExistsMediaCount"] = Convert.ToInt64(rdrds["MediaCount"]);
                                        SereamServerList.Rows[i]["NotExistsFileSize"] = Convert.ToInt64(rdrds["TotalFileSize"]);
                                        //  break;
                                    }
                                    else
                                    {
                                        SereamServerList.Rows[i]["ExistsMediaCount"] = Convert.ToInt64(rdrds["MediaCount"]);
                                        SereamServerList.Rows[i]["ExistsFileSize"] = Convert.ToInt64(rdrds["TotalFileSize"]);

                                    }
                                }


                            }

                        }
                    }
 




                }

                List<tblStreamServer> StreamServerDetails = new List<tblStreamServer>();
                StreamServerDetails = (from DataRow dr in SereamServerList.Rows
                                       select new tblStreamServer()
                                       {
                                           SSServer = dr["SSServer"].ToString(),
                                           StoreID = Convert.ToInt32(dr["StoreID"]),
                                           StoreName = dr["StoreName"].ToString(),
                                           StoreGroupName = dr["StoreGroupName"].ToString(),
                                           DriveCTotal = Convert.ToInt64(dr["DriveCTotal"]),
                                           DriveCFree = Convert.ToInt64(dr["DriveCFree"]),
                                           DriveDTotal = Convert.ToInt64(dr["DriveDTotal"]),
                                           DriveDFree = Convert.ToInt64(dr["DriveDFree"]),
                                           NotExistsMediaCount = Convert.ToInt64(dr["NotExistsMediaCount"]),
                                           ExistsMediaCount = Convert.ToInt64(dr["ExistsMediaCount"]),
                                           NotExistsFileSize = Convert.ToInt64(dr["NotExistsFileSize"]),
                                           ExistsFileSize = Convert.ToInt64(dr["ExistsFileSize"])
                                       }).ToList();



                //List<tblStreamServer> StreamServerDetails = new List<tblStreamServer>();
                //StreamServerDetails = BLL.CommonLogic.ConvertDataTable<tblStreamServer>(SereamServerList);

                return View(StreamServerDetails);
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
           
        }

        public ActionResult ViewServerDetails(string ServerName, string Viewpage)
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                string querySDtnt = "";
                List<tblStreamServer> sslist = new List<tblStreamServer>();
                List<tblDeployStatu> deplist = new List<tblDeployStatu>();
                List<tblDeployLog> litdeplog = new List<tblDeployLog>();

                tblStreamServer stremsr = new tblStreamServer();
                DataTable tblMediaDetails = new DataTable("tblMediaDetails");
                tblMediaDetails.Columns.Add("MediaID", typeof(Int32));
                tblMediaDetails.Columns.Add("FolderName");
                tblMediaDetails.Columns.Add("FolderID", typeof(Int32));
                tblMediaDetails.Columns.Add("Title");
                ViewData["Viewpage"] = Viewpage;
                // DataColumn dtColumn;
                DataRow sserver;
                string MediaIDs = "";
                string FormatIDs = "";
                SqlCommand cmd;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    querySDtnt = "SELECT SVR.SSServer, SVR.IPAddress, SVR.[Status],  isnull(SVR.DriveCTotal,0) DriveCTotal, isnull(SVR.DriveCFree,0) DriveCFree, ";
                    querySDtnt += " IsNull(SVR.DriveDTotal,0) DriveDTotal, IsNull(SVR.DriveDFree,0) DriveDFree, ST.StoreID AS StoreID, ST.StoreName AS StoreName, ";
                    querySDtnt += " ST.StoreGroupID, SG.Name AS StoreGroupName, DS.Name AS DeployScheduleName ";
                    querySDtnt += " FROM tblStreamServer AS SVR ";
                    querySDtnt += " LEFT JOIN tblStoreSubnet AS STSN ON SVR.BelongingSubnet = STSN.Subnet ";
                    querySDtnt += " LEFT JOIN tblStore AS ST ON STSN.Store = ST.StoreID ";
                    querySDtnt += " LEFT JOIN tblStoreGroup AS SG ON ST.StoreGroupID = SG.StoreGroupID ";
                    querySDtnt += " LEFT JOIN tblDeploySchedule AS DS ON SVR.DeploySchedule = DS.DeployScheduleID ";
                    querySDtnt += " WHERE SVR.SSServer = @SSServer ";
                    querySDtnt += " AND SVR.DELFG = 0 AND STSN.DELFG = 0 AND St.DELFG = 0 AND SG.DELFG = 0 AND DS.DELFG = 0 ";


                    cmd = new SqlCommand(querySDtnt, con);
                    cmd.Parameters.AddWithValue("@SSServer", ServerName);
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader rdrs = cmd.ExecuteReader();
                    while (rdrs.Read())
                    {
                        stremsr.SSServer = rdrs["SSServer"].ToString();
                        stremsr.IPAddress = rdrs["IPAddress"].ToString();
                        if (rdrs["Status"] != DBNull.Value)
                        {
                            stremsr.Status = Convert.ToInt16(rdrs["Status"]);
                        }
                        //stremsr.Status =  ? null :  int.Parse(rdrs["Status"]);
                        stremsr.DriveCTotal = Convert.ToInt64(rdrs["DriveCTotal"]);
                        stremsr.DriveCFree = Convert.ToInt64(rdrs["DriveCFree"]);
                        stremsr.DriveDTotal = Convert.ToInt64(rdrs["DriveDTotal"]);
                        stremsr.DriveDFree = Convert.ToInt64(rdrs["DriveDFree"]);
                        stremsr.StoreID = Convert.ToInt32(rdrs["StoreID"]);
                        stremsr.StoreName = rdrs["StoreName"].ToString();
                        stremsr.StoreGroupID = Convert.ToInt32(rdrs["StoreGroupID"]);
                        stremsr.StoreGroupName = rdrs["StoreGroupName"].ToString();
                        stremsr.DeployScheduleName = rdrs["DeployScheduleName"].ToString();

                    }
                    rdrs.Close();

                    querySDtnt = "";
                    querySDtnt = " SELECT isnull(SUM(FileSize),0) AS TotalFileSize ";
                    querySDtnt += " FROM TblDeployStatus AS DS ";
                    querySDtnt += " LEFT JOIN TblMediaFormatInfo AS MFI ON DS.MediaID = MFI.MediaID ";
                    querySDtnt += " AND DS.FormatID = MFI.FormatID ";
                    querySDtnt += " WHERE [DSServer] = @SSServer AND IsExists = 1 ";
                    querySDtnt += "AND MFI.DELFG = 0 AND DS.DELFG = 0";

                    cmd = new SqlCommand(querySDtnt, con);
                    cmd.Parameters.AddWithValue("@SSServer", ServerName);
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader rdrsf = cmd.ExecuteReader();
                    while (rdrsf.Read())
                    {
                        if (stremsr.SSServer != null || stremsr.SSServer != "" || stremsr.SSServer != string.Empty)
                        {
                            stremsr.ExistsFileSize = Convert.ToInt64(rdrsf["TotalFileSize"]);
                        }
                    }
                    rdrsf.Close();

                    ViewData["SSDetails"] = stremsr;


                    querySDtnt = "";
                    querySDtnt = " SELECT FormatID,Name FROM [tblFormat] Where DELFG = 0";
                    querySDtnt += "ORDER BY FormatID ASC";

                    cmd = new SqlCommand(querySDtnt, con);
                    //cmd.Parameters.AddWithValue("@SSServer", ServerName);
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader rdrmf = cmd.ExecuteReader();
                    while (rdrmf.Read())
                    {
                        DataColumn dc = new DataColumn();
                        dc.ColumnName = rdrmf["Name"].ToString();
                        dc.DefaultValue = "UnConfirmed";
                        //tblMediaDetails.Columns.Add(rdrmf["Name"].ToString());
                        tblMediaDetails.Columns.Add(dc);
                        FormatIDs += Convert.ToInt32(rdrmf["FormatID"]) + ",";

                    }
                    rdrmf.Close();
                    FormatIDs = FormatIDs.Remove(FormatIDs.Length - 1);
                    //string MediaIDs = "";
                    querySDtnt = "";

                    querySDtnt = " SELECT M.MediaID AS MediaID, FOL.Name AS FolderName, M.Title, SGF.FolderID ";
                    querySDtnt += " FROM TblMedia AS M ";
                    querySDtnt += " LEFT JOIN tblFolder AS FOL ON M.FolderID = FOL.FolderID ";
                    querySDtnt += " LEFT JOIN tblStore AS ST ON ST.StoreID = @StoreID ";
                    querySDtnt += " LEFT JOIN tblStoreGroupFolder AS SGF ON SGF.FolderID = M.FolderID ";
                    querySDtnt += " AND SGF.StoreGroupID = ST.StoreGroupID ";
                    querySDtnt += " WHERE M.DELFG = 0 AND M.PhysicalDELFG = 0  AND M.ConvertStatus >= 3 ";//VJ 20200603 ConvertStatus Changed 
                    querySDtnt += " AND FOL.DELFG = 0 AND ST.DELFG = 0 AND SGF.DELFG = 0 ";
                    querySDtnt += " AND M.ApprovalStatus >= 3 ORDER BY M.CRTDT DESC ";

                    cmd = new SqlCommand(querySDtnt, con);
                    cmd.Parameters.AddWithValue("@StoreID", stremsr.StoreID);
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader rdrmeddtl = cmd.ExecuteReader();
                    while (rdrmeddtl.Read())
                    {
                        //  tblMediaDetails.Columns.Add(rdrmeddtl["Name"].ToString());
                        MediaIDs += Convert.ToInt32(rdrmeddtl["MediaID"]) + ",";
                        //tblMediaDetails.Columns.Add("MediaID", typeof(Int32));
                        //tblMediaDetails.Columns.Add("FolderName");
                        //tblMediaDetails.Columns.Add("FolderID", typeof(Int32));
                        //tblMediaDetails.Columns.Add("Title");

                        //DataTable tblMediaDetails = new DataTable("tblMediaDetails");

                        //DataRow sserver;
                        sserver = tblMediaDetails.NewRow();
                        sserver["MediaID"] = Convert.ToInt32(rdrmeddtl["MediaID"]);
                        if (rdrmeddtl["FolderID"] != DBNull.Value)
                        {
                            sserver["FolderID"] = Convert.ToInt32(rdrmeddtl["FolderID"]);
                        }
                        //else
                        //{
                        //    sserver["FolderID"] = 0;
                        //}
                        //sserver["FolderID"] = Convert.ToInt32(rdrmeddtl["FolderID"]);
                        sserver["FolderName"] = rdrmeddtl["FolderName"].ToString();
                        sserver["Title"] = rdrmeddtl["Title"].ToString();

                        //for (int i = 4; i < sserver.Table.Columns.Count; i++)
                        //{
                        //    sserver.Table.Columns[i].DefaultValue = "unconfirmed";
                        //}

                        tblMediaDetails.Rows.Add(sserver);

                    }
                    rdrmeddtl.Close();
                    if(MediaIDs.Length > 0)
                    {
                        MediaIDs = MediaIDs.Remove(MediaIDs.Length - 1);
                    }
                    else
                    {
                        MediaIDs = "0";
                    }

                    querySDtnt = "";

                    querySDtnt = " SELECT DS.[DSServer], DS.MediaID, DS.[IsExists], SSF.FormatID,F.Name ";
                    querySDtnt += " FROM tblDeployStatus AS DS ";
                    querySDtnt += " LEFT JOIN tblStreamServerFormat AS SSF ON DS.FormatID = SSF.FormatID ";
                    querySDtnt += " AND DS.[DSServer] = SSF.[SSFServer] ";
                    querySDtnt += "LEFT JOIN tblFormat F On F.FormatID = SSF.FormatID";
                    querySDtnt += " WHERE DS.[DSServer] = @SSServer ";
                    querySDtnt += " AND DS.MediaID IN (" + MediaIDs + ")";
                    querySDtnt += " AND ds.FormatID in (" + FormatIDs + ")";
                    querySDtnt += " AND DS.IsExists = 1 AND DS.DELFG = 0 AND SSF.DELFG = 0 AND F.DELFG = 0";

                    cmd = new SqlCommand(querySDtnt, con);
                    cmd.Parameters.AddWithValue("@SSServer", ServerName);
                    //cmd.Parameters.AddWithValue("@MediaIDs", MediaIDs);
                    //cmd.Parameters.AddWithValue("@FormatIDs", FormatIDs);
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader rdrdsfmtdtl = cmd.ExecuteReader();
                    //deplist
                    while (rdrdsfmtdtl.Read())
                    {
                        tblDeployStatu dpst = new tblDeployStatu();
                        dpst.DSServer = rdrdsfmtdtl["DSServer"].ToString();
                        dpst.FormatName = rdrdsfmtdtl["Name"].ToString();
                        dpst.MediaID = Convert.ToInt32(rdrdsfmtdtl["MediaID"]);

                        if (rdrdsfmtdtl["IsExists"] != DBNull.Value)
                        {
                            dpst.IsExists = Convert.ToBoolean(rdrdsfmtdtl["IsExists"]);
                            //sserver["FolderID"] = Convert.ToInt32(rdrmeddtl["FolderID"]);
                        }
                        if (rdrdsfmtdtl["FormatID"] != DBNull.Value)
                        {
                            dpst.FormatID = Convert.ToInt32(rdrdsfmtdtl["FormatID"]);
                            //sserver["FolderID"] = Convert.ToInt32(rdrmeddtl["FolderID"]);
                        }

                        deplist.Add(dpst);
                    }
                    rdrdsfmtdtl.Close();

                    foreach (tblDeployStatu drlist in deplist)
                    {

                        foreach (DataRow row in tblMediaDetails.Rows)
                        {
                            if (Convert.ToInt64(row["MediaID"]) == drlist.MediaID) // getting the row to edit , change it as you need
                            {

                                for (int i = 4; i < tblMediaDetails.Columns.Count; i++)
                                {
                                    if (row.Table.Columns[i].ColumnName.ToUpper() == drlist.FormatName.ToUpper())
                                    {
                                        if (drlist.FormatID == null || row["FolderID"] == DBNull.Value)
                                        {
                                            row[drlist.FormatName] = "Not applicable for delivery";
                                        }
                                        else
                                        {
                                            if (drlist.IsExists == false)
                                            {
                                                row[drlist.FormatName] = "UnDelivered";
                                            }
                                            else
                                            {
                                                row[drlist.FormatName] = "Delivered";
                                            }
                                        }
                                        break;
                                    }
                                    // sserver.Table.Columns[i].DefaultValue = "unconfirmed";
                                }
                                //row["color"] = someColor;
                            }


                        }
                        //if (recmatch == false)
                        //{
                        //    for (int i = 4; i < tblMediaDetails.Columns.Count; i++)
                        //    {
                        //    }
                        //}
                    }
                    tblMediaDetails.Columns.Remove("FolderID");
                    ViewBag.VidDistStatus = tblMediaDetails;
                    // ViewData["VidDistStatus"] = tblMediaDetails;

                    querySDtnt = "";
                    querySDtnt = " SELECT TOP 50 DL.ElapsedTime, DL.CopiedBytes, DL.[DateTime], M.Title, RC.Content AS ResultMsg ";
                    querySDtnt += " FROM tblDeployLog AS DL ";
                    querySDtnt += " LEFT JOIN tblMedia AS M ON DL.MediaID = M.MediaID ";
                    querySDtnt += " LEFT JOIN tblRobocopyExitcode AS RC ON DL.Result = RC.RobocopyExitcodeID ";
                    querySDtnt += " WHERE [Server] = @SSServer  AND DL.DELFG = 0 AND M.DELFG = 0 AND M.PhysicalDELFG = 0 ";
                    querySDtnt += " AND RC.DELFG = 0 ORDER BY DateTime DESC ";
                    cmd = new SqlCommand(querySDtnt, con);
                    cmd.Parameters.AddWithValue("@SSServer", ServerName);

                    cmd.CommandType = CommandType.Text;

                    //litdeplog

                    SqlDataReader rddeplog = cmd.ExecuteReader();
                    while (rddeplog.Read())
                    {
                        tblDeployLog deplg = new tblDeployLog();
                        deplg.ElapsedTime = Convert.ToInt32(rddeplog["ElapsedTime"]);
                        deplg.CopiedBytes = Convert.ToInt32(rddeplog["CopiedBytes"]);
                        deplg.DateTime = Convert.ToDateTime(rddeplog["DateTime"]);
                        deplg.CRTCD = rddeplog["ResultMsg"].ToString();
                        deplg.UPDCD = rddeplog["Title"].ToString();
                        litdeplog.Add(deplg);
                    }
                    rddeplog.Close();

                    ViewData["VidDelLog"] = litdeplog;
                    //cmd.Parameters.AddWithValue("@SSServer", ServerName);
                    return View(stremsr);
                }
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }

        }

        public ActionResult ViewExpectedDelTime()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                SqlCommand cmd;
                string querySDtnt = "";
                int estimesec = 0;
                List<tblStreamServer> sslist = new List<tblStreamServer>();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    querySDtnt = "SELECT ST.StoreID AS StoreID, ST.[StoreName] AS StoreName, SS.[SSServer],  ";
                    querySDtnt += " SUM(isnull(MFI.FileSize,0)) AS TotalFileSize, ";
                    querySDtnt += " cast(case when SUM(isnull(MFI.FileSize,0)) = 0 then SUM(isnull(MFI.FileSize,0))  ";
                    querySDtnt += " else Round(cast(CAST (SUM(isnull(MFI.FileSize,0)) as float) / 1024 as float) / 1024,2) End as decimal(20,2)) ModTotalFileSize, ";
                    querySDtnt += "isnull(SN.WANBandWidth,0) WANBandWidth,";
                    querySDtnt += " cast(case when isnull(SN.WANBandWidth,0) = 0 then isnull(SN.WANBandWidth,0) ";
                    querySDtnt += " else Round(CAST (isnull(SN.WANBandWidth,0) as float) / 1000 ,2) End as decimal(20,2)) ModWANBandWidth, ";
                    querySDtnt += " cast(round(case when isnull(SN.WANBandWidth,0) = 0 then isnull(SN.WANBandWidth,0) ";
                    querySDtnt += " else SUM(isnull(MFI.FileSize,0)) / isnull(SN.WANBandWidth,0) End,0) as decimal(12,0)) EstTimeReq ";
                    querySDtnt += " FROM tblStreamServer AS SS ";
                    querySDtnt += " LEFT JOIN tblStoreSubnet AS STSB ON SS.BelongingSubnet = STSB.Subnet ";
                    querySDtnt += " LEFT JOIN tblSubnet AS SN ON SN.SubnetID = SS.BelongingSubnet ";
                    querySDtnt += " LEFT JOIN tblStore AS ST ON STSB.Store = ST.StoreID ";
                    querySDtnt += " LEFT JOIN tblStoreGroupFolder AS SGF ON SGF.StoreGroupID = ST.StoreGroupID ";
                    querySDtnt += " INNER JOIN tblMedia AS M ON M.FolderID = SGF.FolderID AND M.DELFG = 0 AND M.PhysicalDELFG = 0 ";
                    querySDtnt += " LEFT JOIN tblStreamServerFormat AS SSF ON SS.[SSServer] = SSF.[SSFServer] ";
                    querySDtnt += " LEFT JOIN tblMediaFormatInfo AS MFI ON SSF.FormatID = MFI.FormatID AND MFI.MediaID = M.MediaID ";
                    querySDtnt += " INNER JOIN tblDeployStatus AS DS ON DS.MediaID = M.MediaID AND DS.FormatID = SSF.FormatID ";
                    querySDtnt += " AND DS.[IsExists] = 0 AND DS.[DSServer] = SS.[SSServer] ";
                    querySDtnt += " Where SS.DELFG = 0 AND STSB.DELFG = 0 AND SN.DELFG = 0 AND ST.DELFG = 0 AND SGF.DELFG = 0 ";
                    querySDtnt += " AND SSF.DELFG = 0 AND MFI.DELFG = 0 AND DS.DELFG = 0 ";
                    querySDtnt += " GROUP BY SS.[SSServer], SN.WANBandWidth, ST.[StoreName], ST.StoreID  ";
                    querySDtnt += " ORDER BY StoreID ASC ";


                    cmd = new SqlCommand(querySDtnt, con);
                    // cmd.Parameters.AddWithValue("@SSServer", ServerName);
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader rdrs = cmd.ExecuteReader();
                    while (rdrs.Read())
                    {
                        tblStreamServer sser = new tblStreamServer();
                        sser.StoreID = Convert.ToInt32(rdrs["StoreID"]);
                        sser.StoreName = rdrs["StoreName"].ToString();
                        sser.SSServer = rdrs["SSServer"].ToString();
                        sser.DriveCTotal = Convert.ToInt64(rdrs["TotalFileSize"]);
                        sser.DriveCFree = Convert.ToInt64(rdrs["ModTotalFileSize"]);
                        sser.DriveDTotal = Convert.ToInt64(rdrs["WANBandWidth"]);
                        sser.DriveDFree = Convert.ToInt64(rdrs["ModWANBandWidth"]);
                        sser.ExistsFileSize = Convert.ToInt32(rdrs["EstTimeReq"]);
                        estimesec += Convert.ToInt32(rdrs["EstTimeReq"]);
                        sslist.Add(sser);
                    }
                    rdrs.Close();

                    ViewData["timereqPred"] = CommonLogic.GetFileDuration(estimesec);
                    return View(sslist);

                }
            }
            catch (Exception ex)
            {
                LogInfo.LogMsg = string.Format("User / Store : {0} Message: {1} ", Session["StoreUserName"].ToString(), ex.Message);
                Log.Error(LogInfo.LogMsg, ex);
                return View("Error", new HandleErrorInfo(ex, LogInfo.ControllerName, LogInfo.ActionName));
            }
 
        }

        public ActionResult ViewInsufficientlyRegistered()
        {
            LogInfo.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            LogInfo.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            LogInfo.MenuClick = LogInfo.ActionName + "_" + LogInfo.ControllerName;
            try
            {
                SqlCommand cmd;
                string querystr = "";

                List<tblSubnet> snlist = new List<tblSubnet>();
                List<tblStreamServer> sslist1 = new List<tblStreamServer>();
                List<tblStreamServer> sslist2 = new List<tblStreamServer>();
                List<tblStore> storelist = new List<tblStore>();
                bool RecExists = false;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    querystr = " SELECT Distinct SN.SubnetID AS SubnetID  ";
                    querystr += " FROM tblSubnet AS SN ";
                    querystr += " LEFT JOIN tblStoreSubnet AS SS ON SS.Subnet = SN.SubnetID  ";
                    querystr += " LEFT JOIN tblStore AS S ON S.StoreID = SS.Store  ";
                    querystr += " WHERE isnull(SN.DELFG,1) = 1   ";
                    querystr += " OR isnull(SS.DELFG,1) = 1  ";
                    querystr += " OR isnull(S.DELFG,1) = 1  ";
                    querystr += " OR S.StoreID IS NULL ";

                    cmd = new SqlCommand(querystr, con);
                    cmd.CommandType = CommandType.Text;
                  
                    SqlDataReader sndr = cmd.ExecuteReader();
                    while (sndr.Read())
                    {
                        RecExists = true;
                        snlist.Add(new tblSubnet { SubnetID = Convert.ToInt32(sndr["SubnetID"]) });
                        ViewData["snlist"] = snlist;
                    }
                    sndr.Close();

                    querystr = "";
                    querystr = " SELECT Distinct SV.[SSServer] ";
                    querystr += " FROM tblStreamServer AS SV ";
                    querystr += " LEFT JOIN tblStreamServerSubnet AS SS ON SV.[SSServer] = SS.SSSServer  ";
                    querystr += " LEFT JOIN tblSubnet AS SN ON SS.Subnet = SN.SubnetID ";
                    querystr += " WHERE isnull(SV.DELFG,1) = 1 ";
                    querystr += " OR isnull(SS.DELFG,1) = 1  ";
                    querystr += " OR isnull(SN.DELFG,1) = 1  ";
                    querystr += " OR SN.SubnetID IS NULL ";

                    cmd = new SqlCommand(querystr, con);
                    cmd.CommandType = CommandType.Text;
              //      con.Open();
                    SqlDataReader sslist1dr = cmd.ExecuteReader();
                    while (sslist1dr.Read())
                    {
                        RecExists = true;
                        sslist1.Add(new tblStreamServer { SSServer = sslist1dr["SSServer"].ToString() });
                        ViewData["sslist1"] = sslist1;
                    }
                    sslist1dr.Close();

                    querystr = "";
                    querystr = " SELECT Distinct SV.[SSServer] ";
                    querystr += " FROM tblStreamServer AS SV ";
                    querystr += " LEFT JOIN tblSubnet AS SN ON SV.BelongingSubnet = SN.SubnetID  ";
                    querystr += " WHERE isnull(SV.DELFG,1) = 1 ";
                    querystr += " OR isnull(SN.DELFG,1) = 1  ";
                    querystr += " OR SN.SubnetID IS NULL ";

                    cmd = new SqlCommand(querystr, con);
                    cmd.CommandType = CommandType.Text;
                   // con.Open();
                    SqlDataReader sslist2dr = cmd.ExecuteReader();
                    while (sslist2dr.Read())
                    {
                        RecExists = true;
                        sslist2.Add(new tblStreamServer { SSServer = sslist2dr["SSServer"].ToString() });
                        ViewData["sslist2"] = sslist2;
                    }
                    sslist2dr.Close();

                    querystr = "";
                    querystr = " SELECT Distinct ST.[StoreID] AS StoreID, ST.[StoreName] AS StoreName ";
                    querystr += " FROM tblStore AS ST ";
                    querystr += " LEFT JOIN tblStoreSubnet AS SS ON SS.Store = ST.StoreID ";
                    querystr += " LEFT JOIN tblSubnet AS SN ON SS.Subnet = SN.SubnetID ";
                    querystr += " WHERE isnull(ST.DELFG,1) = 1 ";
                    querystr += " OR isnull(SS.DELFG,1) = 1  ";
                    querystr += " OR isnull(SN.DELFG,1) = 1  ";
                    querystr += " OR SN.SubnetID IS NULL ";

                    cmd = new SqlCommand(querystr, con);
                    cmd.CommandType = CommandType.Text;
                 //   con.Open();
                    SqlDataReader storelistdr = cmd.ExecuteReader();
                    while (storelistdr.Read())
                    {
                        RecExists = true;
                        storelist.Add(new tblStore
                        {
                            StoreID = Convert.ToInt32(storelistdr["StoreID"]),
                            StoreName = storelistdr["StoreName"].ToString()
                        });

                        ViewData["storelist"] = storelist;
                    }
                    storelistdr.Close();

                }
                ViewData["RecExists"] = RecExists;
                return View("InsufficientlyRegistered");
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
