using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.DocumentCore.IncomingModel;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentTagsController : ApiController
    {
        /// <summary>
        /// Получение тегов документа.
        /// Зависит от выставленных позиций
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns>Теги документа</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tagProc = DmsResolver.Current.Get<IDocumentTagService>();
            return new JsonResult(tagProc.GetTags(cxt, id), this);
        }

        /// <summary>
        /// Изменение списка тега документа
        /// Зависит от выставленных позиций
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись плана работы над документом</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentTags model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tagProc = DmsResolver.Current.Get<IDocumentTagService>();
            tagProc.ModifyDocumentTags(cxt, model);
            return Get(model.DocumentId);
        }
    }
}
