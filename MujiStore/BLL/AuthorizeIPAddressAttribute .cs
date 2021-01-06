using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Web.Mvc;
using MujiStore.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using MujiStore.BLL;


namespace MujiStore.BLL
{
    public class AuthorizeIPAddressAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        string CS = ConfigurationManager.ConnectionStrings["cnstr"].ConnectionString;
        string IpFilterStatus = System.Configuration.ConfigurationManager.AppSettings["IpFilter"];//ConfigurationManager.ConnectionStrings["IpFilter"].ConnectionString;
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext context)
        {
            if (IpFilterStatus == "true")
            {
                //string ipAddress = HttpContext.Current.Request.UserHostAddress;
                //string ipAddress = "192.168.43.51";
                string ipAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
                if (ipAddress == "::1")
                {
                    MujiStore.BLL.IPAddressDtl Ipadd = new BLL.IPAddressDtl();
                    ipAddress = Ipadd.GetIPAddress();
                }
                Boolean blGip = true;
                if (!IsIpAddressAllowed(ipAddress.ToString().Trim()))
                {
                    blGip = false;
                    context.Result = new System.Web.Mvc.HttpStatusCodeResult(403, MujiStore.Resources.Resource.Error403Msg);
                    var viewResult = new ViewResult();
                    viewResult.ViewName = "~/Views/Shared/_Unauthorized.cshtml";
                    context.HttpContext.Response.StatusCode = 403;
                    context.Result = viewResult;
                }
            
            }
        }
        private bool IsIpAddressAllowed(string IpAddress)
        {
            bool IPAllowed = false;
            String SubnetID = HttpContext.Current.Session["SubnetID"].ToString();
            if (SubnetID != "-1")
            {
                IPAllowed =  true;
            }
            return IPAllowed;
            //if (!string.IsNullOrWhiteSpace(IpAddress))
            //{
            //    List<string> addresses = new List<string>();

            //    String strIpAddress = "";
            //    int intStrLength = 0;
            //    string query = "";
            //    query = "SELECT StoreID,SNIPAddress StoreIPAddress ";
            //    query += " FROM tblStore S ";
            //    query += " JOIN tblStoreSubnet SS on SS.Store = S.StoreID ";
            //    query += " JOIN tblSubnet SN on SN.SubnetID = SS.Subnet ";
            //    query += " WHERE S.DELFG=0 AND SS.DELFG = 0  AND SN.DELFG = 0";

            //    using (SqlConnection con = new SqlConnection(CS))
            //    {
            //        SqlCommand cmd = new SqlCommand(query, con);
            //        cmd.CommandType = CommandType.Text;
            //        con.Open();
            //        SqlDataReader rdr = cmd.ExecuteReader();
            //        while (rdr.Read())
            //        {
            //            //1st Add by Default
            //            addresses.Add(rdr["StoreIPAddress"].ToString());

            //            //If Ip Adderss in Database global IP set like 192.168.1.0
            //            //.0 then Add 1 to 255
            //            strIpAddress = rdr["StoreIPAddress"].ToString();
            //            if (strIpAddress.Length > 2)
            //            {
            //                intStrLength = strIpAddress.Length;
            //                strIpAddress = strIpAddress.Substring(intStrLength - 2, 2); // Get .0 from right
            //                if (strIpAddress == ".0")
            //                {
            //                    //First find  Total Length

            //                    string strIpPart = "";

            //                    // Assign again value
            //                    strIpAddress = rdr["StoreIPAddress"].ToString();
            //                    intStrLength = strIpAddress.Length;

            //                    //Need to Add
            //                    strIpPart = strIpAddress.Substring(0, intStrLength - 2);
            //                    for (int i = 1; i < 256; i++)
            //                    {
            //                        addresses.Add(strIpPart + "." + i.ToString());
            //                        //Console.WriteLine("Value of i: {0}", i);
            //                    }

            //                }
            //                else // Not matched end of .0 then add as it is in database
            //                {
            //                    addresses.Add(rdr["StoreIPAddress"].ToString());
            //                }

            //            }
            //        }
            //    }

            //    return addresses.Where(a => a.Trim().Equals(IpAddress, StringComparison.InvariantCultureIgnoreCase)).Any();
            //}
            //return false;
        }
    }

    public class IPAddressDtl
    {
        public string GetIPAddress()
        {
            string IpAddress = "";
            System.Net.IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            System.Net.IPAddress ipAddress = host.AddressList.Where(ips => ips.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();
            IpAddress = ipAddress.ToString().Trim();
            return IpAddress;
        }
    }

    public class RolesAttribute : AuthorizeAttribute
    {
        public RolesAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }

      

        
    }

    public class MUJICustomAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // If they are authorized, handle accordingly
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                // Otherwise redirect to your specific authorized area
                filterContext.Result = new RedirectResult("~/VideoFiles/ViewUploadDetails");
            }
        }
    }

    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            // check  sessions here
            if (HttpContext.Current.Session["username"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
                return;
            }
            BLL.CommonLogic.SetCultureInfo();
            base.OnActionExecuting(filterContext);
        }
    }
}