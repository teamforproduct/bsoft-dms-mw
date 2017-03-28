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
        public async Task<IHttpActionResult> IsSendAllForExecution()
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ISettings>();
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
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ISettings>();
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
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ISettings>();
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
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ISettings>();
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
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ISettings>();
                var tmpItem = tmpService.GetFulltextRefreshTimeout(context);
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
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ISettings>();
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
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ISettings>();
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
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ISettings>();
                var tmpItem = tmpService.GetClearTrashDocumentsTimeoutMinuteForClear(context);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

    }
}