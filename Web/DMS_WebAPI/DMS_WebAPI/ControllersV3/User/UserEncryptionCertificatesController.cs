using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Diagnostics;
using BL.Model.Common;
using BL.Model.EncryptionCore.FrontModel;
using BL.Model.EncryptionCore.Filters;
using BL.Model.SystemCore;
using BL.Logic.EncryptionCore.Interfaces;
using System.Web;
using BL.Model.EncryptionCore.IncomingModel;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Ключи ЕЦП
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserSignCertificatesController : ApiController
    {

        Stopwatch stopWatch = new Stopwatch();

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
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (paging == null) paging = new UIPaging();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IEncryptionService>();
            var tmpItems = tmpService.GetCertificates(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
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
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IEncryptionService>();
            var tmpItem = tmpService.GetCertificate(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
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
            if (!stopWatch.IsRunning) stopWatch.Restart();
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
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
            if (!stopWatch.IsRunning) stopWatch.Restart();
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
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumEncryptionActions.DeleteEncryptionCertificate, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}