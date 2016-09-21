using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Actions;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.Enums;
using BL.Model.DocumentCore.IncomingModel;
using BL.Logic.EncryptionCore.Interfaces;
using System.Web;
using System.IO;
using System;

namespace DMS_WebAPI.Controllers.Encryption
{
    [Authorize]
    [RoutePrefix("api/v2/EncryptionActions")]
    public class EncryptionActionsController : ApiController
    {
        /// <summary>
        /// Получить отчет pdf документа перед подписанием
        /// </summary>
        /// <param name="model">model</param>>
        /// <returns></returns>
        [Route("VerifyPdf")]
        [HttpPost]
        public IHttpActionResult VerifyPdf()
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();

            var res = false;

            try
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                using (var memoryStream = new MemoryStream())
                {
                    file.InputStream.CopyTo(memoryStream);
                    var model = memoryStream.ToArray();

                    res = (bool)encryptionProc.ExecuteAction(EnumEncryptionActions.VerifyPdf, ctx, model);
                }
            }
            catch(Exception ex)
            {
                res = false;
            }
            return new JsonResult(res, this);
        }
    }
}
