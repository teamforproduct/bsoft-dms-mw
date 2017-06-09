using BL.Logic.AdminCore.Interfaces;
using DMS_WebAPI.ControllersV3.Utilities;
using DMS_WebAPI.Models;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DMS_WebAPI.Utilities
{
    public static class RenderPartialView
    {
        public static string ControllerName { get { return "WebPartialViewController"; } }
        public static string MailWithCallToAction { get { return "~/Views/WebPartialView/MailWithCallToAction.cshtml"; } }


        public static string RenderPartialViewToString<T>(this T model, string partialViewName) where T : class
        {
            using (var controller = new WebPartialViewController())
            {
                controller.ViewData.Model = model;

                controller.ControllerContext = new ControllerContext(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData() { Values = { { "controller", ControllerName } } }), controller);

                using (StringWriter sw = new StringWriter())
                {
                    var view = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialViewName).View;

                    view.Render(new ViewContext(controller.ControllerContext, view, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw), sw);

                    return sw.ToString();
                }
            }
        }
    }
}