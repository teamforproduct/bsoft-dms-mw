using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentRestrictedSendListsController : ApiController
    {
        /// <summary>
        /// Добавление записи ограничительного списка
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененный документ</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentRestrictedSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var restrictedSendListId = docProc.AddRestrictedSendList(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Добавление ограничительного списка по стандартному списку
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененный документ</returns>
        public IHttpActionResult Put([FromBody]ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.AddRestrictedSendListByStandartSendLists(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }
        /*
        /// <summary>
        /// Добавление ограничительного списка
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Измененный документ</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentRestrictedSendList model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.UpdateRestrictedSendList(cxt, model);
            return new Results.JsonResult(null, this);
        }
        */
        /// <summary>
        /// Удаление записи ограничительного списка
        /// </summary>
        /// <param name="id">ИД записи ограничительного списка</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.DeleteRestrictedSendList(cxt, id);
            return new JsonResult(null, this);
        }
        
    }
}
