using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. Связи.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentLinkController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetLinkedDocumentIds(context, Id);
            var res = new JsonResult(items, this);
            return res;
        }

        /// <summary>
        /// Возвращает список ИД связанных документов по ИД документа TODO зачем нужно?
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Links)]
        [ResponseType(typeof(List<int>))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет связи между документами
        /// </summary>
        /// <param name="model"></param>
        /// <returns>массив ИД связанных доков</returns>
        [HttpPost]
        [Route(Features.Links)]
        [ResponseType(typeof(List<int>))]
        public async Task<IHttpActionResult> Post([FromBody]AddDocumentLink model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.AddDocumentLink, model, model.CurrentPositionId);
                   var res = new JsonResult(null, this);
                   return GetById(context, model.DocumentId);
               });
        }

        /// <summary>
        /// Удаляет связь между документами
        /// </summary>
        /// <param name="Id">ИД связи</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Links + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.DeleteDocumentLink, Id);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

    }
}
