using BL.Model.Enums;
using DMS_WebAPI.Controllers.System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DMS_WebAPI.Utilities
{
    public static class RenderPartialView
    {
        public static string ControllerName { get { return "WebPartialViewController"; } }
        public static string PartialViewNameChangeLoginAgentUserVerificationEmail { get { return "~/Views/WebPartialView/ChangeLoginAgentUserVerificationEmail.cshtml"; } }
        public static string RestorePasswordAgentUserVerificationEmail { get { return "~/Views/WebPartialView/RestorePasswordAgentUserVerificationEmail.cshtml"; } }
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