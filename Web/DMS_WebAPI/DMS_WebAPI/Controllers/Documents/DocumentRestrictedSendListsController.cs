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
    public class DocumentRestrictedSendListsController : ApiController
    {
        /// <summary>
        /// Получение записи ограничительного списка
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Запись ограничительного списка</returns>
        private IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            return new JsonResult(docProc.GetRestrictedSendList(cxt, id), this);
        }
        /// <summary>
        /// Получение записей ограничительного списка
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns>Записи ограничительного списка</returns>
        private IHttpActionResult GetByDocument(int documentId)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            return new JsonResult(docProc.GetRestrictedSendLists(cxt, documentId), this);
        }
        /// <summary>
        /// Добавление записи ограничительного списка
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененные записи ограничительного списка</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentRestrictedSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var newId = (int)docProc.ExecuteAction(EnumDocumentActions.AddDocumentRestrictedSendList, cxt, model);
            return Get(newId);
        }

        /// <summary>
        /// Добавление ограничительного списка по стандартному списку
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная записи ограничительного списка</returns>
        public IHttpActionResult Put([FromBody]ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.AddByStandartSendListDocumentRestrictedSendList, cxt, model);
            return GetByDocument(model.DocumentId);
        }
        /// <summary>
        /// Удаление записи ограничительного списка
        /// </summary>
        /// <param name="id">ИД записи ограничительного списка</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            int docId = (int)docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentRestrictedSendList, cxt, id);
            return GetByDocument(docId);
        }
        
    }
}
