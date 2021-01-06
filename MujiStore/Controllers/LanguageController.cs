using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Threading;
using MujiStore.BLL;

namespace MujiStore.Controllers
{
    public class LanguageController : Controller
    {
        // GET: Language
        public ActionResult Index()
        {
            CommonLogic.SetCultureInfo();
            return View();
        }

        public ActionResult Change(string LanguageAbbrevation)
        {

            if (LanguageAbbrevation != null)
            {
                Session["CreateSpecificCulture"] = LanguageAbbrevation;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Session["CreateSpecificCulture"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["CreateSpecificCulture"].ToString());

            }
            else
            {
                Session["CreateSpecificCulture"] = "ja";
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Session["CreateSpecificCulture"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["CreateSpecificCulture"].ToString());
            }

            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = LanguageAbbrevation;
            Response.Cookies.Add(cookie);

            return View("Index");

        }

    }
}