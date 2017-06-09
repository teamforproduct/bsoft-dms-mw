using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
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
    /// Документы. Ограничительный список рассылки.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentTagController : WebApiController
    {
        /// <summary>
        /// Возвращает список тегов по ИД документа
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Tags)]
        [ResponseType(typeof(List<FrontDocumentTag>))]
        public async Task<IHttpActionResult> GetByDocumentId(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var items = docProc.GetDocumentTags(context, new FilterDocumentTag { DocumentId = new List<int> { Id } });
                var res = new JsonResult(items, this);
                return res;
            });
        }
        /// <summary>
        /// Возвращает счетчик списка тегов по ИД документа
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [ResponseType(typeof(int))]
        [HttpGet]
        [Route("{Id:int}/" + Features.Tags + "/Counter")]
        public async Task<IHttpActionResult> GetByDocumentIdCounter(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var items = docProc.GetDocumentTagsCounter(context, new FilterDocumentTag { DocumentId = new List<int> { Id } });
                var res = new JsonResult(items, this);
                return res;
            });
        }

        /// <summary>
        /// Добавляет список тегов для документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Tags)]
        public async Task<IHttpActionResult> Post([FromBody]ModifyDocumentTags model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.ExecuteDocumentAction(context, EnumActions.ModifyDocumentTags, model, model.CurrentPositionId);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

    }
}


    

