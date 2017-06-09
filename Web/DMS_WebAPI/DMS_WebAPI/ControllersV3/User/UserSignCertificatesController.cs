using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.EncryptionCore.Interfaces;
using BL.Model.Common;
using BL.Model.EncryptionCore.Filters;
using BL.Model.EncryptionCore.FrontModel;
using BL.Model.EncryptionCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Ключи ЕЦП
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserSignCertificatesController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IEncryptionService>();
            var tmpItem = tmpService.GetCertificate(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список сертификатов
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SignCertificates)]
        [ResponseType(typeof(List<FrontEncryptionCertificate>))]
        public async Task<IHttpActionResult> Get([FromUri]FilterEncryptionCertificate filter, [FromUri]UIPaging paging)
        {
            if (paging == null) paging = new UIPaging();

            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IEncryptionService>();
                var tmpItems = tmpService.GetCertificates(context, filter, paging);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает сертификат по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SignCertificates + "/{Id:int}")]
        [ResponseType(typeof(FrontEncryptionCertificate))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавление нового сертификата.
        /// Из-за того что необходимо передать файл, все остальные параметры перадаються через GET параметры.
        /// multipart/form-data
        /// Все аналогично добавлению файла к документу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.SignCertificates)]
        public async Task<IHttpActionResult> Post([FromBody]AddEncryptionCertificate model)
        {
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;

            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                model.AgentId = context.CurrentAgentId;
                var tmpItem = Action.ExecuteEncryptionAction(context, EnumActions.AddEncryptionCertificate, model);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует название сертификата
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.SignCertificates)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyEncryptionCertificate model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.ExecuteEncryptionAction(context, EnumActions.ModifyEncryptionCertificate, model);
                return GetById(context, model.Id);
            });
        }

        /// <summary>
        /// Удаляет сертификат
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.SignCertificates + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.ExecuteEncryptionAction(context, EnumActions.DeleteEncryptionCertificate, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }
    }
}