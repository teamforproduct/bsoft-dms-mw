using System.Web.Mvc;

namespace DMS_WebAPI.Controllers.System
{
    public class HomeController : Controller
    {
        public ActionResult Index(int? id)
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
