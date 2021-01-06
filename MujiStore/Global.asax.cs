using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Threading;
using System.Globalization;

namespace MujiStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string CS = ConfigurationManager.ConnectionStrings["cnstr"].ConnectionString;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();


            // test comment

        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {

            HttpCookie cookie = HttpContext.Current.Request.Cookies["Language"];
            if (cookie != null && cookie.Value != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
            }
            else // By Default
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            }
        }

       
        protected void Session_Start()
        {
            string query = "";
            bool isValidStore = false;
            Session["CreateSpecificCulture"] = "ja";
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Session["CreateSpecificCulture"].ToString());
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["CreateSpecificCulture"].ToString());

            //Session["IPAddress"] = "192.168.1.107";
            string strIp = System.Web.HttpContext.Current.Request.UserHostAddress;
            if (strIp == "::1")
            {
                MujiStore.BLL.IPAddressDtl Ipadd = new BLL.IPAddressDtl();
                strIp = Ipadd.GetIPAddress();
            }
            Session["IPAddress"] = strIp;

            Session["SubnetID"] = "-1";
            Session["SNIPAddress"] = "-1";
            Session["WANBandWidth"] = "0";

            List <MujiStore.Models.tblSubnet> snetlist = new List<MujiStore.Models.tblSubnet>();
            snetlist = MujiStore.BLL.CommonLogic.getSubNetDetailsIP(Session["IPAddress"].ToString());
            if (snetlist.Count > 0)
            {
                foreach (MujiStore.Models.tblSubnet items in snetlist)
                {
                    Session["SubnetID"] = Convert.ToInt32(items.SubnetID).ToString();
                    Session["SNIPAddress"] = items.SNIPAddress.ToString();
                    Session["WANBandWidth"] = items.WANBandWidth.ToString();
                    break;
                }
            }
           
            //string ipAddress = Session["IPAddress"].ToString();
            //string[] values = ipAddress.Split('.'); ;
            //int lastPort = Convert.ToInt16(values[3].ToString());
            //string ipcuripAdd = values[0] + "." + values[1] + "." + values[2];
            //UserorStoreName = ipAddress;
            using (SqlConnection con = new SqlConnection(CS))
            {
                query = "SELECT StoreID,SNIPAddress StoreIPAddress,StoreName,PARSENAME(SNIPAddress, 4)+'.'+";
                query += "PARSENAME(SNIPAddress, 3)+'.'+PARSENAME(SNIPAddress, 2) ipcuripAdd,";
                query += "PARSENAME(SNIPAddress, 1) lastPort FROM tblStore S ";
                query += "JOIN tblStoreSubnet SS on SS.Store = S.StoreID ";
                query += "JOIN tblSubnet SN on SN.SubnetID = SS.Subnet ";
                query += "WHERE SN.SubnetID = " + Convert.ToInt32(Session["SubnetID"]);
                query += " AND S.DELFG=0 AND SS.DELFG = 0  AND SN.DELFG = 0 ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Session["StoreName"] = rdr["StoreName"].ToString();
                    Session["StoreUserName"] = rdr["StoreName"].ToString();
                    Session["StoreIPAddress"] = rdr["StoreIPAddress"].ToString();
                    Session["StoreID"] = rdr["StoreID"].ToString();
                    isValidStore = true;
                    break;

                    //if(rdr["lastPort"].ToString() != "::1")
                    //{ 
                    //    if (Convert.ToInt16(rdr["lastPort"].ToString()) == 0)
                    //    {
                    //        if (ipcuripAdd == rdr["ipcuripAdd"].ToString())
                    //        {
                    //            if (lastPort >= Convert.ToInt16(rdr["lastPort"].ToString()) && lastPort <= 255)
                    //            {
                    //                Session["StoreName"] = rdr["StoreName"].ToString();
                    //                Session["StoreUserName"] = rdr["StoreName"].ToString();
                    //                Session["StoreIPAddress"] = rdr["StoreIPAddress"].ToString();
                    //                Session["StoreID"] = rdr["StoreID"].ToString();
                    //                isValidStore = true;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (ipAddress == rdr["StoreIPAddress"].ToString())
                    //    {
                    //        Session["StoreName"] = rdr["StoreName"].ToString();
                    //        Session["StoreUserName"] = rdr["StoreName"].ToString();
                    //        Session["StoreIPAddress"] = rdr["StoreIPAddress"].ToString();
                    //        Session["StoreID"] = rdr["StoreID"].ToString();
                    //        isValidStore = true;
                    //        break;
                    //    }

                    //}
                }

                if (isValidStore == false)
                {
                    Session["StoreName"] = "Unknown";//"Unknown";
                    Session["StoreUserName"] = "Unknown"; 
                    Session["StoreIPAddress"] = Session["IPAddress"];
                    Session["StoreID"] = 0;
                }
            }
           
        }
   

  
        public void Session_End()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ja");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja");
            Session.Clear();
           
        }
    }

    //public partial class tblSubnet
    //{
    //    public int SubnetID { get; set; }
    //    public string SNIPAddress { get; set; }
    //    public string SubnetMask { get; set; }
    //    public double WANBandWidth { get; set; }
    //    public double LANBandWidth { get; set; }
    //    public bool DELFG { get; set; }
    //}
}
