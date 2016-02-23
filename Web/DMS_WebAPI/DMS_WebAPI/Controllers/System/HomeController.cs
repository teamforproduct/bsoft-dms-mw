using System.Web.Mvc;
using BL.Logic.Context;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.DocumentCore;

namespace DMS_WebAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
