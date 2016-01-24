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
            var docProc = DmsResolver.Current.Get<IDocumentProcessor>();
            docProc.SaveDocument(new DefaultContext
            {
                CurrentEmployee = new BL.Model.Users.Employee
                {
                    Token = "1"
                }
            }, new BaseDocument());


            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
