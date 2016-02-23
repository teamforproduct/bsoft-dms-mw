using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentSendListStagesController : ApiController
    {
        /// <summary>
        /// Добавление этапа плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentSendListStage model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            bool isLastStage = docProc.AddSendListStage(cxt, model);
            return Get(model.DocumentId, isLastStage);
        }

        /// <summary>
        /// Удаление этапа плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri]ModifyDocumentSendListStage model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            docProc.DeleteSendListStage(cxt, model);

            return Get(model.DocumentId);
        }

        /// <summary>
        /// Получение плана работы над документом
        /// </summary>
        /// <param name="DocumentId">ИД документа</param>
        /// <param name="isLastStage"></param>
        /// <returns>План работы над документом</returns>
        private IHttpActionResult Get(int DocumentId, bool isLastStage = false)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            return new JsonResult(docProc.GetSendListStage(cxt, DocumentId, isLastStage), this);
        }
    }
}
