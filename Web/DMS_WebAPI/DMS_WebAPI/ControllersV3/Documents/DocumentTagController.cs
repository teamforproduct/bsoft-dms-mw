using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
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
                var docProc = DmsResolver.Current.Get<IDocumentTagService>();
                var items = docProc.GetTags(context, Id);
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
                   var tmpItem = Action.Execute(context, EnumDocumentActions.ModifyDocumentTags, model, model.CurrentPositionId);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

    }
}
