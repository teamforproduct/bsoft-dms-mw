using System.Web.Mvc;
using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
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
