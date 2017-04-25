using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Threading.Tasks;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Системные настройки
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.System)]
    public class SystemSettingsController : WebApiController
    {
        /// <summary>
        /// Возвращает значение настройки: 
        /// Если флаг TRUE, то при создании новой должности устанавливается рассылка на все должности для исполнения, 
        /// в противном случае (FALSE) рассылка устанавливается в дефолт
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/IsSendAllForExecution")]
        public async Task<IHttpActionResult> IsSendAllForExecution()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISettingValues>();
                var tmpItem = tmpService.GetSubordinationsSendAllForExecution(context);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// Если флаг TRUE, то при создании новой должности устанавливается рассылку на все должности для сведения, 
        /// в противном случае (FALSE) рассылка устанавливается в дефолт
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/IsSendAllForInforming")]
        public async Task<IHttpActionResult> IsSendAllForInforming()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISettingValues>();
                var tmpItem = tmpService.GetSubordinationsSendAllForInforming(context);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/IsDigitalSignatureIsUseCertificateSign")]
        public async Task<IHttpActionResult> IsDigitalSignatureIsUseCertificateSign()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISettingValues>();
                var tmpItem = tmpService.GetDigitalSignatureIsUseCertificateSign(context);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/IsDigitalSignatureIsUseInternalSign")]
        public async Task<IHttpActionResult> IsDigitalSignatureIsUseInternalSign()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISettingValues>();
                var tmpItem = tmpService.GetDigitalSignatureIsUseInternalSign(context);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/FulltextRefreshTimeout")]
        public async Task<IHttpActionResult> GetFulltextRefreshTimeout()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISettingValues>();
                var tmpItem = tmpService.GetFulltextRefreshTimeout();
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/FulltextRowLimit")]
        public async Task<IHttpActionResult> GetFulltextRowLimit()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISettingValues>();
                var tmpItem = tmpService.GetFulltextRowLimit();
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/AutoplanTimeout")]
        public async Task<IHttpActionResult> GetAutoplanTimeoutMinute()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISettingValues>();
                var tmpItem = tmpService.GetAutoplanTimeoutMinute(context);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/ClearTrashDocumentsTimeout")]
        public async Task<IHttpActionResult> GetClearTrashDocumentsTimeoutMinute()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISettingValues>();
                var tmpItem = tmpService.GetClearTrashDocumentsTimeoutMinute(context);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Settings + "/ClearTrashDocumentsTimeoutMinuteForClear")]
        public async Task<IHttpActionResult> GetClearTrashDocumentsTimeoutMinuteForClear()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISettingValues>();
                var tmpItem = tmpService.GetClearTrashDocumentsTimeoutMinuteForClear(context);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }
    }
}