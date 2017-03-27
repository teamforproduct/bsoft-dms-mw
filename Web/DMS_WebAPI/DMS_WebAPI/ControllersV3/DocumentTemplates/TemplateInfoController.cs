using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.DocumentTemplates
{
    /// <summary>
    /// Шаблоны документов
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Templates)]
    public class TemplateInfoController : ApiController
    {
        /// <summary>
        /// Возвращает список шаблонов документов
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontMainTemplateDocument>))]
        public IHttpActionResult Get([FromUri]FullTextSearch ftSearch, [FromUri]FilterTemplateDocument filter, UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItems = tmpService.GetMainTemplateDocument(ctx, ftSearch, filter, paging);
            var res = new JsonResult(tmpItems, this);
            return res;
        }


        /// <summary>
        /// Возвращает шаблон по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontTemplateDocument))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItem = tmpService.GetTemplateDocument(ctx, Id);
            var metaData = tmpService.GetModifyMetaData(ctx, tmpItem);
            var res = new JsonResult(tmpItem, metaData, this);
            return res;
        }

        /// <summary>
        /// Добавляет шаблон
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddTemplateDocument model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.AddTemplateDocument, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Создает копию шаблон
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info + "/Duplicate")]
        public IHttpActionResult Duplicate([FromBody]Item model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.CopyTemplateDocument, model.Id);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует шаблон
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyTemplateDocument model)
        {
            Action.Execute(EnumDocumentActions.ModifyTemplateDocument, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет шаблон
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDocumentActions.DeleteTemplateDocument, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
