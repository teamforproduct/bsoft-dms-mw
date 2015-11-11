using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsoft_dms_mw.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Logic.Auth_login(string.Empty,string.Empty);
        //    Logic.GetDocument1();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}