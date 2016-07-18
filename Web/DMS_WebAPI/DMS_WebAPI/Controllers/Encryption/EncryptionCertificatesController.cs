﻿using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Enums;
using BL.Model.SystemCore;
using System.Web;
using BL.Model.EncryptionCore.Filters;
using BL.Logic.EncryptionCore.Interfaces;
using BL.Model.EncryptionCore.IncomingModel;

namespace DMS_WebAPI.Controllers.Encryption
{
    [Authorize]
    [RoutePrefix("api/v2/EncryptionCertificates")]
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
            if (paging == null)
            {
                paging = new UIPaging();
            }

            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();
            var items = encryptionProc.GetCertificates(ctx, filter, paging);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();
            var res = encryptionProc.GetCertificate(ctx, id);

            return new JsonResult(res, this);
        }

        /// <summary>
        /// Добавление нового сертификата
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromUri]AddEncryptionCertificate model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;

            var itemId = (int)encryptionProc.ExecuteAction(EnumEncryptionActions.AddEncryptionCertificate, ctx, model);
            return Get(itemId);
        }

        /// <summary>
        /// Изменить название сертификата
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, ModifyEncryptionCertificate model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();

            var itemId = (int)encryptionProc.ExecuteAction(EnumEncryptionActions.ModifyEncryptionCertificate, ctx, model);

            return Get(itemId);
        }

        /// <summary>
        /// Удаление сертификата
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();
            encryptionProc.ExecuteAction(EnumEncryptionActions.DeleteEncryptionCertificate, ctx, id);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Экспорт сертификата
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Export/{id}")]
        [HttpPost]
        public IHttpActionResult PostExport(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();
            var item = encryptionProc.ExecuteAction(EnumEncryptionActions.ExportEncryptionCertificate, ctx, id);

            return new JsonResult(item, this);
        }
    }
}
