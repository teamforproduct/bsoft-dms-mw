using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore;
using BL.Model.Common;
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

namespace DMS_WebAPI.ControllersV3.DocumentTemplates
{
    /// <summary>
    /// Шаблоны документов. Доступы.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Templates)]
    public class TemplateAccessesController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItem = tmpService.GetTemplateDocumentAccess(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список
        /// </summary>
        /// <param name="Id">Id шаблона</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Accesses)]
        [ResponseType(typeof(List<FrontTemplateDocumentAccess>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterTemplateDocumentAccess filter)
        {
            if (filter == null) filter = new FilterTemplateDocumentAccess();
            filter.TemplateId =  Id ;

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItems = tmpService.GetTemplateDocumentAccesses(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }


        /// <summary>
        /// Возвращает по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Accesses + "/{Id:int}")]
        [ResponseType(typeof(FrontTemplateDocumentAccess))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Accesses)]
        public IHttpActionResult Post([FromBody]AddTemplateDocumentAccess model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.AddTemplateDocumentAccess, model);
            return GetById(context, tmpItem);
        }

        /// <summary>
        /// Корректирует
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Accesses)]
        public IHttpActionResult Put([FromBody]ModifyTemplateDocumentAccess model)
        {
            Action.Execute(EnumDocumentActions.ModifyTemplateDocumentAccess, model);
            return GetById(context, model.Id);
        }

        /// <summary>
        /// Удаляет
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Accesses + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDocumentActions.DeleteTemplateDocumentAccess, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
