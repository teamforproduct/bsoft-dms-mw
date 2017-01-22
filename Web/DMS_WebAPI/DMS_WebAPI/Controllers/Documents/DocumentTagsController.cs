using BL.Logic.DocumentCore.Interfaces;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentTagsController : ApiController
    {
        /// <summary>
        /// Получение тегов документа.
        /// Зависит от выставленных позиций use V3
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns>Теги документа</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tagProc = DmsResolver.Current.Get<IDocumentTagService>();
            return new JsonResult(tagProc.GetTags(ctx, id), this);
        }

        /// <summary>
        /// Изменение списка тега документа use V3
        /// Зависит от выставленных позиций
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись плана работы над документом</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentTags model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentTags, ctx, model);
            return Get(model.DocumentId);
        }
    }
}
