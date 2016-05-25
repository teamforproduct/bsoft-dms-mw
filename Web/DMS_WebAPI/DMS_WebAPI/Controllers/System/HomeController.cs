using System.Web.Mvc;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Utilities;

namespace DMS_WebAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int? id)
        {
            ViewBag.Title = "Home Page";

            //TODO удалить в релизе
            if (id.HasValue)
            {
                var sdbw = new SystemDbWorker();
                var lic = sdbw.GetLicenceInfo(id.GetValueOrDefault());
                ViewBag.RegCode = new SystemInfo().GetProgramRegCode(lic);
            }

            return View();
        }
    }
}
