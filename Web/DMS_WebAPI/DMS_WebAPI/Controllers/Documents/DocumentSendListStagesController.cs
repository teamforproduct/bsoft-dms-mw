using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentSendListStagesController : ApiController
    {
        /// <summary>
        /// Получение плана работы над документом
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns>План работы над документом</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            return Get(ctx, id);
        }

        /// <summary>
        /// Получение плана работы над документом
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="DocumentId">ИД документа</param>
        /// <param name="isLastStage"></param>
        /// <returns>План работы над документом</returns>
        private IHttpActionResult Get(IContext ctx, int DocumentId, bool isLastStage = false)
        {
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            return new JsonResult(docProc.GetSendListStage(ctx, DocumentId, isLastStage), this);
        }

        /// <summary>
        /// Добавление этапа плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentSendListStage model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var isLastStage = (bool)docProc.ExecuteAction(EnumDocumentActions.AddDocumentSendListStage, ctx, model);
            return Get(ctx, model.DocumentId, isLastStage);
        }

        /// <summary>
        /// Удаление этапа плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri]ModifyDocumentSendListStage model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentSendListStage, ctx, model);
            return Get(ctx, model.DocumentId);
        }
    }
}
