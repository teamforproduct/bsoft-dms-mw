using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Diagnostics;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Системные настройки
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.System)]
    public class SystemSettingsController : ApiController
    {
        /// <summary>
        /// Возвращает значение настройки: 
        /// Если флаг TRUE, то при создании новой должности устанавливается рассылка на все должности для исполнения, 
        /// в противном случае (FALSE) рассылка устанавливается в дефолт
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/IsSendAllForExecution")]
        public IHttpActionResult IsSendAllForExecution()
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetSubordinationsSendAllForExecution(cxt);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// Если флаг TRUE, то при создании новой должности устанавливается рассылку на все должности для сведения, 
        /// в противном случае (FALSE) рассылка устанавливается в дефолт
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/IsSendAllForInforming")]
        public IHttpActionResult IsSendAllForInforming()
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetSubordinationsSendAllForInforming(cxt);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/IsDigitalSignatureIsUseCertificateSign")]
        public IHttpActionResult IsDigitalSignatureIsUseCertificateSign()
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetDigitalSignatureIsUseCertificateSign(cxt);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/IsDigitalSignatureIsUseInternalSign")]
        public IHttpActionResult IsDigitalSignatureIsUseInternalSign()
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetDigitalSignatureIsUseInternalSign(cxt);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/FulltextRefreshTimeout")]
        public IHttpActionResult GetFulltextRefreshTimeout()
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetFulltextRefreshTimeout(cxt);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/AutoplanTimeout")]
        public IHttpActionResult GetAutoplanTimeoutMinute()
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetAutoplanTimeoutMinute(cxt);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/ClearTrashDocumentsTimeout")]
        public IHttpActionResult GetClearTrashDocumentsTimeoutMinute()
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetClearTrashDocumentsTimeoutMinute(cxt);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/ClearTrashDocumentsTimeoutMinuteForClear")]
        public IHttpActionResult GetClearTrashDocumentsTimeoutMinuteForClear()
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetClearTrashDocumentsTimeoutMinuteForClear(cxt);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}