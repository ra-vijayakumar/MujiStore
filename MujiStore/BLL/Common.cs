using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;

using System.Data.SqlClient;
using System.IO;
using System.Web.Mvc;
using MujiStore.Models;
using System.Reflection;
using System.Threading;
using System.Globalization;

namespace MujiStore.BLL
{
    public class CommonLogic
    {
        static string CS = ConfigurationManager.ConnectionStrings["cnstr"].ConnectionString;
       
        internal static string ModifiedFileName(string OrigFileName)
        {
            var ext = Path.GetExtension(OrigFileName);
            var name = Path.GetFileNameWithoutExtension(OrigFileName);
            var fileName = name + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
            return fileName;
        }

        internal static string GetFileNameMp4(string OrigFileName)
        {
            var ext = Path.GetExtension(OrigFileName);
            var name = Path.GetFileNameWithoutExtension(OrigFileName);
            var fileName = name + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4";
            return fileName;
        }

        internal static string GetFileNameActualMp4(string OrigFileName)
        {
            var ext = Path.GetExtension(OrigFileName);
            var name = Path.GetFileNameWithoutExtension(OrigFileName);
            var fileName = name + ".mp4";
            return fileName;
        }
        internal static void Log_info( string menuclick, string Comments)
        {
            string query = "";
           
            string storename = HttpContext.Current.Session["StoreName"].ToString();
            string ipaddress = HttpContext.Current.Session["IPAddress"].ToString();
            string countryname = "Japan";
            string user;
            if (HttpContext.Current.Session["UserName"] == null || HttpContext.Current.Session["UserName"] == "")
            {
                user = "";
            }
            else
            {
                user = HttpContext.Current.Session["UserName"].ToString();
            }
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    query = "Insert into tblVideoLogReport(StoreName,CountryName,IPAddress,UserName,MenuClick,DELFG,CRTDT,CRTCD,Comments) Values ";
                    query += "(@storename,@countryname,@ipaddress,@UserName,@menuclick,@DELFG,@CRTDT,@CRTCD,@Comments);";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@storename", storename);
                    cmd.Parameters.AddWithValue("@countryname", countryname);
                    cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                    cmd.Parameters.AddWithValue("@UserName", user);
                    cmd.Parameters.AddWithValue("@menuclick", menuclick);
                    cmd.Parameters.AddWithValue("@DELFG", "0");
                    cmd.Parameters.AddWithValue("@CRTDT", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CRTCD", user);
                    cmd.Parameters.AddWithValue("@Comments", Comments);
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    int result = cmd.ExecuteNonQuery();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Not inserted due to error");
            }

        }

        internal static SelectList ToSelectList(DataTable table, string valueField, string textField)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (DataRow row in table.Rows)
            {
                list.Add(new SelectListItem()
                {
                    Text = row[textField].ToString(),
                    Value = row[valueField].ToString()
                });
            }

            return new SelectList(list, "Value", "Text");
        }

        internal static List<SelectListItem> GetPageSize()
        {
            List<SelectListItem> lstpagesizeList = new List<SelectListItem>()
            {
            new SelectListItem() { Value="1", Text= "1" },
            new SelectListItem() { Value="3", Text= "3" },
            new SelectListItem() { Value="4", Text= "4" },
             new SelectListItem() { Value="5", Text= "5" },
             new SelectListItem() { Value="10", Text= "10" },
             new SelectListItem() { Value="15", Text= "15" },
             new SelectListItem() { Value="25", Text= "25" },
             new SelectListItem() { Value="50", Text= "50" },
            };

            return lstpagesizeList;

        }

        internal static int EnsureLngValue(string value, Int16 def)
        {
            int result;
            int intParsed;
            
            if (string.IsNullOrEmpty(value))
            {
                result = def;
            }
            else if (int.TryParse(value.Trim(), out intParsed))
            {
                result = Convert.ToInt16(value);
            }
            else
            {
                result = def;
            }
       
            return result;

        }

        public static string GetFileDuration(int duration)
        {
            string sdur = "";
            if ((duration > 3600))
            {
                sdur = (duration + ((duration / 3600) + " 時間"));
                duration = (duration % 3600);
            }

            if ((duration > 60))
            {
                sdur = (sdur + ((duration / 60)) + " 分");
                duration = (duration % 60);
            }

            sdur = (sdur + (duration + " 秒"));
            return sdur;
        }

        public static string ApplySubnetMask(string ipaddr, string netmask)
        {
            string ipAddress = "";
            string[] ipaddrst = ipaddr.Split('.');
            string[] netmaskst = netmask.Split('.');
            for (int i = 0; i < ipaddrst.Length; i++)
            {
                ipAddress += (Convert.ToInt16(ipaddrst[i]) & Convert.ToInt16(netmaskst[i])).ToString() + ".";
            }

            return ipAddress.Substring(0, ipAddress.Length - 1);
        }
        public static List<string> GetFileExtn(string type)
        {
            List<string> list = new List<string>();
            if(type.ToLower() == "video")
            {
                list.Add(".mp4");
                list.Add(".3g2");
                list.Add(".3gp");
                list.Add(".avi");
                list.Add(".flv");
                list.Add(".h264");
                list.Add(".m4v");
                list.Add(".mkv");
                list.Add(".mov");
                list.Add(".mpg");
                list.Add(".mpeg");
                list.Add(".rm");
                list.Add(".swf");
                list.Add(".vob");
                list.Add(".wmv");
                list.Add(".mp3");
                list.Add(".mp2");
            }
            return list;
        }

        public static List<tblSubnet> getSubNetDetailsIP(string IPAddress)
        {
            
            string querySDtl = "";

            //string ipAddress = "192.168.43.51";
            string[] ipAddressSplit = IPAddress.Split('.');

            // querySDtl = "SELECT SubnetID from [dbo].[tblSubnet]  where DELFG = 0";
            //querySDtl += "and SNIPAddress = '" + CommonLogic.ApplySubnetMask(subnet.SNIPAddress, {0} + "' and SubnetMask = '" + {0 } + "'";
            //querySDtl += "and SNIPAddress = '" + CommonLogic.ApplySubnetMask(subnet.SNIPAddress, "SubnetMask") + "'";

            List<tblSubnet> snetlist = new List<tblSubnet>();

            List<tblStore> stelist = new List<tblStore>();
            tblMedia media = new tblMedia();

            

           
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    
                        querySDtl = "SELECT top 1 PARSENAME(SNIPAddress, 4) as part1,PARSENAME(SNIPAddress, 3) as part2, ";
                        querySDtl += "PARSENAME(SNIPAddress, 2) as part3,PARSENAME(SNIPAddress, 1) as part4, ";
                        querySDtl += "SubnetID,SNIPAddress,SubnetMask,WANBandWidth,LANBandWidth ";
                        querySDtl += " From [dbo].[tblSubnet]  where DELFG = 0 ";
                        querySDtl += "and PARSENAME(SNIPAddress, 4) = '" + ipAddressSplit[0] + "' ";
                        querySDtl += "and PARSENAME(SNIPAddress, 3) = '" + ipAddressSplit[1] + "' ";
                        querySDtl += "and PARSENAME(SNIPAddress, 2) = '" + ipAddressSplit[2] + "' ";
                        querySDtl += "and " + Convert.ToInt16(ipAddressSplit[3]) + " between ";
                        querySDtl += " cast(PARSENAME(SNIPAddress, 1) as int) and cast(255 as int) ";
                        querySDtl += " order by cast(PARSENAME(SNIPAddress, 1) as int) desc";
                //querySDtl += " and SNIPAddress = '" + CommonLogic.ApplySubnetMask(IPAddress, items.SubnetMask) + "' and SubnetMask ='" + items.SubnetMask + "'";

                SqlCommand cmd = new SqlCommand(querySDtl, con);
                        cmd.CommandType = CommandType.Text;

                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            tblSubnet snet = new tblSubnet();
                            snet.SubnetID = Convert.ToInt32(rdr["SubnetID"]);
                            snet.SNIPAddress = rdr["SNIPAddress"].ToString();
                            snet.SubnetMask = rdr["SubnetMask"].ToString();
                            snet.WANBandWidth = Convert.ToInt64(rdr["WANBandWidth"]);
                            snet.LANBandWidth = Convert.ToInt64(rdr["LANBandWidth"]);
                            snetlist.Add(snet);
                        }
                        rdr.Close();
                   


                    //SqlConnection con1 = new SqlConnection(CS);
                    //querySDtnt = "select distinct SubnetMask  from [dbo].[tblSubnet] where DELFG = 0";

                }
           
            return snetlist;
        }

        public static List<tblSubnet> getSubNetDetails(string IPAddress)
        {
            string querySDtnt = "";
            string querySDtl = "";
            // querySDtl = "SELECT SubnetID from [dbo].[tblSubnet]  where DELFG = 0";
            //querySDtl += "and SNIPAddress = '" + CommonLogic.ApplySubnetMask(subnet.SNIPAddress, {0} + "' and SubnetMask = '" + {0 } + "'";
            //querySDtl += "and SNIPAddress = '" + CommonLogic.ApplySubnetMask(subnet.SNIPAddress, "SubnetMask") + "'";
            List<tblSubnet> subnetMask = new List<tblSubnet>();
            List<tblSubnet> snetlist = new List<tblSubnet>();

            List<tblStore> stelist = new List<tblStore>();
            tblMedia media = new tblMedia();

            using (SqlConnection con = new SqlConnection(CS))
            {

                //SqlConnection con1 = new SqlConnection(CS);
                querySDtnt = "select distinct SubnetMask  from [dbo].[tblSubnet] where DELFG = 0";
                SqlCommand cmd = new SqlCommand(querySDtnt, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    tblSubnet snet = new tblSubnet();
                    snet.SubnetMask = rdr["SubnetMask"].ToString();
                    subnetMask.Add(snet);
                }

            }

            if (subnetMask.Count > 0)
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    foreach (tblSubnet items in subnetMask)
                    {
                        querySDtl = "SELECT SubnetID,SNIPAddress,SubnetMask,WANBandWidth,LANBandWidth ";
                        querySDtl += " From [dbo].[tblSubnet]  where DELFG = 0";
                        querySDtl += " and SNIPAddress = '" + CommonLogic.ApplySubnetMask(IPAddress, items.SubnetMask) + "' and SubnetMask ='" + items.SubnetMask + "'";

                        SqlCommand cmd = new SqlCommand(querySDtl, con);
                        cmd.CommandType = CommandType.Text;

                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            tblSubnet snet = new tblSubnet();
                            snet.SubnetID = Convert.ToInt32(rdr["SubnetID"]);
                            snet.SNIPAddress = rdr["SNIPAddress"].ToString();
                            snet.SubnetMask = rdr["SubnetMask"].ToString();
                            snet.WANBandWidth = Convert.ToInt64(rdr["WANBandWidth"]);
                            snet.LANBandWidth = Convert.ToInt64(rdr["LANBandWidth"]);
                            snetlist.Add(snet);
                        }
                        rdr.Close();
                    }

                   
                    //SqlConnection con1 = new SqlConnection(CS);
                    //querySDtnt = "select distinct SubnetMask  from [dbo].[tblSubnet] where DELFG = 0";

                }
            }
            return snetlist;
        }

        public static List<tblDeployStatu> GetDSWithSS(int MediaID, int SubNetID)
        {
            List<tblDeployStatu> deplist = new List<tblDeployStatu>();
            string query = "";
            using (SqlConnection con = new SqlConnection(CS))
            {
                query = "SELECT DS.[DSServer], SS.IPAddress AS ServerIP, DS.FormatID, F.Name AS FormatName ";
                query += "FROM tblDeployStatus AS DS ";
                query += "LEFT JOIN [tblFormat] AS F ON DS.FormatID = F.FormatID ";
                query += "LEFT JOIN tblStreamServer AS SS ON DS.[DSServer] = SS.SSServer ";
                query += "WHERE DS.MediaID =@MediaID ";
                query += "AND DS.[IsExists] = 1 ";
                query += "AND SS.BelongingSubnet = @SubNetID AND DS.DELFG = 0 AND F.DELFG = 0 AND SS.DELFG = 0 ";


                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MediaID", MediaID);
                cmd.Parameters.AddWithValue("@SubNetID", SubNetID);

                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    tblDeployStatu Depstat = new tblDeployStatu();
                    Depstat.DSServer = rdr["DSServer"].ToString();
                    Depstat.IPAddress = rdr["ServerIP"].ToString();
                    Depstat.FormatID = Convert.ToInt32(rdr["FormatID"]);
                    Depstat.FormatName = rdr["FormatName"].ToString();
                    Depstat.Recommend = true;
                    deplist.Add(Depstat);
                }
            }

            return deplist;
        }

        public static List<tblDeployStatu> GetDSWithSNSS(int MediaID, int SubNetID)
        {
            List<tblDeployStatu> deplist = new List<tblDeployStatu>();
            string query = "";
            using (SqlConnection con = new SqlConnection(CS))
            {
                query = "SELECT DS.[DSServer], SS.IPAddress AS ServerIP, SS.BelongingSubnet AS ServerSubnet, ";
                query += "DS.FormatID, F.Name AS FormatName, F.RequiredBandWidth, SN.WANBandWidth AS ServerWANBandWidth  ";
                query += "FROM tblDeployStatus AS DS ";
                query += "LEFT JOIN [tblFormat] AS F ON DS.FormatID = F.FormatID ";
                query += "LEFT JOIN tblStreamServer AS SS ON DS.[DSServer] = SS.[SSServer] ";
                query += "LEFT JOIN tblSubnet AS SN ON SS.BelongingSubnet = SN.SubnetID  ";
                query += "WHERE DS.[DSServer] IN ";
                query += " ( ";
                query += "SELECT[SSSServer] FROM[tblStreamServerSubnet] WHERE Subnet = @SubNetID ";
                query += "AND DELFG = 0 ";
                query += " ) ";
                query += "AND MediaID = @MediaID AND [IsExists] = 1 AND SS.BelongingSubnet <> @SubNetID  ";

                query += "AND DS.DELFG = 0 AND F.DELFG = 0 AND SS.DELFG = 0 AND SN.DELFG = 0 ";
                query += "ORDER BY F.RequiredBandWidth ASC ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MediaID", MediaID);
                cmd.Parameters.AddWithValue("@SubNetID", SubNetID);

                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    tblDeployStatu Depstat = new tblDeployStatu();
                    Depstat.DSServer = rdr["DSServer"].ToString();
                    Depstat.IPAddress = rdr["Title"].ToString();
                    Depstat.FormatID = Convert.ToInt32(rdr["FormatID"]);
                    Depstat.RequiredBandWidth = Convert.ToInt64(rdr["RequiredBandWidth"]);
                    Depstat.FormatName = rdr["FormatName"].ToString();
                    if ((Convert.ToInt32(rdr["ServerWANBandWidth"]) > Convert.ToInt32(rdr["RequiredBandWidth"])) &&
                        (Convert.ToInt32(HttpContext.Current.Session["WANBandWidth"].ToString()) > Convert.ToInt32(rdr["RequiredBandWidth"])))
                    {
                        Depstat.Recommend = true;
                    }
                    else
                    {
                        Depstat.Recommend = false;
                    }

                    deplist.Add(Depstat);
                }
            }

            return deplist;
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        // pro.SetValue(obj, dr[column.ColumnName], null);
                        pro.SetValue(obj, dr[column.ColumnName]);
                    else
                        continue;
                }
            }
            return obj;
        }

        public static void SetCultureInfo()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(HttpContext.Current.Session["CreateSpecificCulture"].ToString());
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(HttpContext.Current.Session["CreateSpecificCulture"].ToString());
        }

        public static List<SelectListItem> FillFolderList()
        {
            string query = "";
            var list = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                //SqlCommand cmd = new SqlCommand("Select VideoId,Title,Description,Video,Thumbnail,FoderID from [tblVideoDemo]", con);
                // query = "Select FolderID,Name from tblFolder where Delfg = 0 Order by FolderID";
                //query = ";WITH x AS ";
                //query += "(";
                //query += " SELECT FolderID, Name, ParentID, [level] = 1 ";
                //query += " FROM tblFolder WHERE ParentID = -1 AND   DELFG=0 ";
                //query += " UNION ALL ";
                //query += " SELECT t.FolderID, t.Name, t.ParentID, [level] = x.[level] + 1 ";
                //query += " FROM x INNER JOIN dbo.tblFolder AS t ";
                //query += " ON t.ParentID = x.FolderID ";
                //query += " ) ";
                //query += " SELECT FolderID, name,REPLICATE('  ', [level] - 1) +name 'TreeStructure' ,ParentID, [level] FROM x ";
                //query += " where FolderID in (select FolderID from tblFolder where DELFG = 0 ";
                //query += " ) ";
                //query += " ORDER BY [level] ";
                //query += " OPTION (MAXRECURSION 32) ";
                query = " WITH foldercte(FolderId, [Name], ParentID, LEVEL, treepath,treepath1) AS ";
                query += " ( SELECT FolderID AS FolderId, [Name], ParentID, 1 AS LEVEL, ";
                query += " CAST([Name] AS VARCHAR(8000)) AS treepath,CAST([Name] AS VARCHAR(8000)) AS treepath1 ";
                query += " FROM [tblFolder] WHERE ParentID = -1 ";
                query += " UNION ALL ";
                query += " SELECT d.FolderID AS FolderId, d.[Name], d.ParentID, ";
                query += " foldercte.LEVEL + 1 AS LEVEL, ";
                query += " CAST(foldercte.treepath + ' -> ' +CAST(d.[Name] AS VARCHAR(1024)) AS VARCHAR(8000)) AS treepath, ";
                query += " CAST(SPACE(5 * foldercte.LEVEL + 1)  +CAST(d.[Name] AS VARCHAR(1024)) AS VARCHAR(8000)) AS treepath1 ";
                query += " FROM [tblFolder] d INNER JOIN foldercte ON foldercte.FolderId = d.ParentID) ";
                query += " SELECT * FROM foldercte where FolderId in (select FolderID from [tblFolder] where DELFG = 0) ORDER BY treepath";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    list.Add(new SelectListItem { Text = rdr["treepath1"].ToString().Replace(' ', Convert.ToChar(160)), Value = Convert.ToInt32(rdr["FolderId"]).ToString() });

                }
            }

            // list.Add(new SelectListItem { Text = "Instructor", Value = "Instructor" });
            //list.Add(new SelectListItem { Text = "Student", Value = "Student" });


            return list;


        }
    }


    
}