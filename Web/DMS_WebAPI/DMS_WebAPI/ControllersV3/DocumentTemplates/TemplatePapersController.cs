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
    /// Шаблоны документов. Бумажные носители.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Templates)]
    public class TemplatePapersController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<ITemplateService>();
            var tmpItem = tmpService.GetTemplatePaper(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список бумажных носителей
        /// </summary>
        /// <param name="Id">ИД шаблона</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Papers)]
        [ResponseType(typeof(List<FrontTemplatePaper>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterTemplatePaper filter)
        {
            if (filter == null) filter = new FilterTemplatePaper();
            filter.TemplateId = Id;

            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ITemplateService>();
                var tmpItems = tmpService.GetTemplatePapers(context, filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }


        /// <summary>
        /// Возвращает бумажный носитель по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Papers + "/{Id:int}")]
        [ResponseType(typeof(FrontTemplatePaper))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет бумажный носитель
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Papers)]
        public async Task<IHttpActionResult> Post([FromBody]AddTemplatePaper model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpItem = Action.ExecuteDocumentAction(context, EnumActions.AddTemplatePaper, model);
                   return GetById(context, tmpItem);
               });
        }

        /// <summary>
        /// Корректирует бумажный носитель
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Papers)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyTemplatePaper model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.ExecuteDocumentAction(context, EnumActions.ModifyTemplatePaper, model);
                   return GetById(context, model.Id);
               });
        }

        /// <summary>
        /// Удаляет бумажный носитель
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Papers + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.ExecuteDocumentAction(context, EnumActions.DeleteTemplatePaper, Id);
                   var tmpItem = new FrontDeleteModel(Id);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

    }
}
