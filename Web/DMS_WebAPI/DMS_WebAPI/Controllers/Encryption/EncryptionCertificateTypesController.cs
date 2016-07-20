using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.EncryptionCore.Filters;
using BL.Logic.EncryptionCore.Interfaces;
using BL.Model.EncryptionCore.IncomingModel;

namespace DMS_WebAPI.Controllers.Encryption
{
    [Authorize]
    public class EncryptionCertificateTypesController : ApiController
    {
        /// <summary>
        /// Получить список типов сертификатов
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]FilterEncryptionCertificateType filter, UIPaging paging)
        {
            if (paging == null)
            {
                paging = new UIPaging();
            }

            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();
            var items = encryptionProc.GetCertificateTypes(ctx, filter, paging);

            var res = new JsonResult(items, this);

            res.Paging = paging;

            return res;
        }

        /// <summary>
        /// Получить тип
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();
            var res = encryptionProc.GetCertificateType(ctx, id);

            return new JsonResult(res, this);
        }

        /// <summary>
        /// Добавление нового типа сертификата
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyEncryptionCertificateType model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();

            var itemId = (int)encryptionProc.ExecuteAction(EnumEncryptionActions.AddEncryptionCertificateType, ctx, model);
            return Get(itemId);
        }

        /// <summary>
        /// Изменить название типа
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, ModifyEncryptionCertificateType model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();

            var itemId = (int)encryptionProc.ExecuteAction(EnumEncryptionActions.ModifyEncryptionCertificateType, ctx, model);

            return Get(itemId);
        }

        /// <summary>
        /// Удаление тип сертификата
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();
            encryptionProc.ExecuteAction(EnumEncryptionActions.DeleteEncryptionCertificateType, ctx, id);
            return new JsonResult(null, this);
        }
    }
}
