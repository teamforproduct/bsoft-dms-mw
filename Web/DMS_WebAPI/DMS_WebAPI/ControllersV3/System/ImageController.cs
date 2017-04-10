using System.IO;
using System.Web.Http;
using System.Web.Mvc;
using DMS_WebAPI.Utilities;

namespace DMS_WebAPI.ControllersV3.System
{
    public class ImageController : WebApiController
    {
        public ActionResult GetDmsFile()
        {

                //string filename = "tenderReport.xls";

                //byte[] filedata = fs.ToArray();
                //string contentType = "application/vnd.ms-excel";

                //var cd = new System.Net.Mime.ContentDisposition
                //{
                //    FileName = filename,
                //    Inline = true,
                //};

                //Response.AppendHeader("Content-Disposition", cd.ToString());

                //return File(filedata, contentType);
            return null;
        }
    }
}