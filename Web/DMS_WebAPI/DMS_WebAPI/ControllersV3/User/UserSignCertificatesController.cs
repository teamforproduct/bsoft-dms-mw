using BL.CrossCutting.DependencyInjection;
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
    public class UserSignCertificatesController : ApiController
    {
        /// <summary>
        /// Возвращает список сертификатов
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SignCertificates)]
        [ResponseType(typeof(List<FrontEncryptionCertificate>))]
        public IHttpActionResult Get([FromUri]FilterEncryptionCertificate filter, UIPaging paging)
        {
            if (paging == null) paging = new UIPaging();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IEncryptionService>();
            var tmpItems = tmpService.GetCertificates(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает сертификат по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SignCertificates + "/{Id:int}")]
        [ResponseType(typeof(FrontEncryptionCertificate))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IEncryptionService>();
            var tmpItem = tmpService.GetCertificate(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
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
        public IHttpActionResult Post([FromBody]AddEncryptionCertificate model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.AgentId = ctx.CurrentAgentId;
            var tmpItem = Action.Execute(EnumEncryptionActions.AddEncryptionCertificate, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует название сертификата
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.SignCertificates)]
        public IHttpActionResult Put([FromBody]ModifyEncryptionCertificate model)
        {
            Action.Execute(EnumEncryptionActions.ModifyEncryptionCertificate, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет сертификат
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.SignCertificates + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumEncryptionActions.DeleteEncryptionCertificate, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }
    }
}