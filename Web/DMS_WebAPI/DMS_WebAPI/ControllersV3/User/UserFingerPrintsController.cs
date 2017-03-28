using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.Common;
using BL.Model.SystemCore;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Отпечатки браузера
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserFingerprintsController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var tmpItem = webService.GetUserFingerprint(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список отпечатков браузера
        /// </summary>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Fingerprints)]
        [ResponseType(typeof(List<FrontAspNetUserFingerprint>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterAspNetUserFingerprint filter)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var webService = DmsResolver.Current.Get<WebAPIService>();
                var user = webService.GetUser(context, context.CurrentAgentId);

                if (filter == null) filter = new FilterAspNetUserFingerprint();
                filter.UserIDs = new List<string> { user.Id };

                var tmpItems = webService.GetUserFingerprints(filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает отпечаток по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Fingerprints + "/{Id:int}")]
        [ResponseType(typeof(FrontAspNetUserFingerprint))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Создает новый отпечаток
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Fingerprints)]
        public async Task<IHttpActionResult> Post([FromBody]AddAspNetUserFingerprint model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var webService = DmsResolver.Current.Get<WebAPIService>();
                var user = webService.GetUser(context, context.CurrentAgentId);
                model.UserId = user.Id;
                var tmpItem = webService.AddUserFingerprint(context, model);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует отпечаток
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Fingerprints)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyAspNetUserFingerprint model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var webService = DmsResolver.Current.Get<WebAPIService>();
                var user = webService.GetUser(context, context.CurrentAgentId);
                model.UserId = user.Id;
                webService.UpdateUserFingerprint(model);
                return GetById(context, model.Id);
            });
        }

        /// <summary>
        /// Удаляет отпечаток
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Fingerprints + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var webService = DmsResolver.Current.Get<WebAPIService>();
                webService.DeleteUserFingerprint(Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }


        /// <summary>
        /// Возвращает параметр из профиля "Использовать безопасный вход"
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Fingerprints + "/Enabled")]
        [ResponseType(typeof(List<FrontAspNetUserFingerprint>))]
        public async Task<IHttpActionResult> GetEnabled()
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var webService = DmsResolver.Current.Get<WebAPIService>();
                var user = webService.GetUser(context, context.CurrentAgentId);
                var res = new JsonResult(user.IsFingerprintEnabled, this);
                return res;
            });
        }

        /// <summary>
        /// Устанавливает "Использовать безопасный вход"
        /// </summary>
        /// <param name="model">параметры фильтрации</param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Fingerprints + "/Enabled")]
        public async Task<IHttpActionResult> SetEnabled([FromBody]ModifyAspNetUserFingerprintEnabled model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var webService = DmsResolver.Current.Get<WebAPIService>();
                webService.ChangeFingerprintEnabled(model.Enabled);
                var res = new JsonResult(null, this);
                return res;
            });
        }
    }
}