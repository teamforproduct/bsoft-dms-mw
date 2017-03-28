using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
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

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. Сохраненные фильтры документов.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentSavedFilterController : ApiController
    {

        private IHttpActionResult GetById(IContext context, int Id)
        {
            var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
            var item = docProc.GetSavedFilter(context, Id);
            var res = new JsonResult(item, this);
            return res;
        }

        /// <summary>
        /// Возвращает список всех сохраненных фильтров документов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SavedFilters + "/All")]
        [ResponseType(typeof(List<FrontDocumentSavedFilter>))]
        public async Task<IHttpActionResult> Get()
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
                   var items = docProc.GetSavedFilters(context, new FilterDocumentSavedFilter { IsOnlyCurrentUser = false });
                   var res = new JsonResult(items, this);
                   return res;
               });
        }

        /// <summary>
        /// Возвращает список сохраненных фильтров документов для текущего пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SavedFilters + "/CurrentUser")]
        [ResponseType(typeof(List<FrontDocumentSavedFilter>))]
        public async Task<IHttpActionResult> GetOnlyCurrentUser()
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
                   var items = docProc.GetSavedFilters(context, new FilterDocumentSavedFilter { IsOnlyCurrentUser = true });
                   var res = new JsonResult(items, this);
                   return res;
               });
        }

        /// <summary>
        /// Возвращает сохраненный фильтр документов по ИД
        /// </summary>
        /// <param name="Id">ИД фильтра</param>
        /// <returns>запись пункта плана</returns>
        [HttpGet]
        [Route(Features.SavedFilters + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentSavedFilter))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет сохраненный фильтр документов плана TODO переделать входящие модели
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.SavedFilters)]
        public async Task<IHttpActionResult> Post([FromBody]AddDocumentSavedFilter model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   var tmpItem = Action.Execute(context, EnumDocumentActions.AddSavedFilter, model);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Измененяет сохраненный фильтр документов
        /// </summary>
        /// <param name="model">Модель для обновления</param>
        /// <returns>Обновленный пункт плана</returns>
        [HttpPut]
        [Route(Features.SavedFilters)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyDocumentSavedFilter model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   var tmpItem = Action.Execute(context, EnumDocumentActions.ModifySavedFilter, model);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Удаляет сохраненный фильтр документов
        /// </summary>
        /// <param name="Id">ИД сохраненного фильтра документов</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.SavedFilters + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.DeleteSavedFilter, Id);
                   var tmpItem = new FrontDeleteModel(Id);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

    }
}
