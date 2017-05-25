using BL.CrossCutting.DependencyInjection;
using BL.Model.Common;
using BL.Model.SystemCore;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using Microsoft.AspNet.Identity;
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
    public class UserFingerprintsController : WebApiController
    {
        private IHttpActionResult GetById(int Id)
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
        public IHttpActionResult Get([FromUri] FilterAspNetUserFingerprint filter)
        {
            //!ASYNC
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var user = webService.GetUserById(User.Identity.GetUserId());

            if (filter == null) filter = new FilterAspNetUserFingerprint();
            filter.UserIDs = new List<string> { user.Id };

            var tmpItems = webService.GetUserFingerprints(filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает отпечаток по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Fingerprints + "/{Id:int}")]
        [ResponseType(typeof(FrontAspNetUserFingerprint))]
        public IHttpActionResult Get(int Id)
        {
            //!ASYNC
            return GetById(Id);
        }

        /// <summary>
        /// Создает новый отпечаток
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Fingerprints)]
        public IHttpActionResult Post([FromBody]AddAspNetUserFingerprint model)
        {
            //!ASYNC
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var user = webService.GetUserById(User.Identity.GetUserId());
            model.UserId = user.Id;
            var tmpItem = webService.AddUserFingerprint(model);
            return GetById(tmpItem);
        }

        /// <summary>
        /// Корректирует отпечаток
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Fingerprints)]
        public IHttpActionResult Put([FromBody]ModifyAspNetUserFingerprint model)
        {
            //!ASYNC
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var user = webService.GetUserById(User.Identity.GetUserId());
            model.UserId = user.Id;
            webService.UpdateUserFingerprint(model);
            return GetById( model.Id);
        }

        /// <summary>
        /// Удаляет отпечаток
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Fingerprints + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            //!ASYNC
            var webService = DmsResolver.Current.Get<WebAPIService>();
            webService.DeleteUserFingerprint(Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }


        /// <summary>
        /// Возвращает параметр из профиля "Использовать безопасный вход"
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Fingerprints + "/Enabled")]
        [ResponseType(typeof(List<FrontAspNetUserFingerprint>))]
        public IHttpActionResult GetEnabled()
        {
            //!ASYNC
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var user = webService.GetUserById(User.Identity.GetUserId());
            var res = new JsonResult(user.IsFingerprintEnabled, this);
            return res;
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
            //!ASYNC
            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.ChangeFingerprintEnabled(User.Identity.GetUserId(), model.Enabled);
            return new JsonResult(null, this);
        }
    }
}