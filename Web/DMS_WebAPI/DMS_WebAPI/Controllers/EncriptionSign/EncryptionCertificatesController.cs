using BL.Model.EncryptionCore.Filters;
using BL.Model.EncryptionCore.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using System.Collections.Generic;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Encryption
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "EncryptionCertificates")]
    public class EncryptionCertificatesController : ApiController
    {
        /// <summary>
        /// Получить список сертификатов
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]FilterEncryptionCertificate filter, UIPaging paging)
        {
            var items = new List<FrontEncryptionCertificate>();
            var res = new JsonResult(items, this);
            res.Paging = paging;
            return res;
        }

        /// <summary>
        /// Получить сертификат
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Get(int id)
        {
            var res = new FrontEncryptionCertificate();
            return new JsonResult(res, this);
        }

    }
}