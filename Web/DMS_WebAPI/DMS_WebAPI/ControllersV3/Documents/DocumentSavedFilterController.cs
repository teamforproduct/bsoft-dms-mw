using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        /// Возвращает список всех сохраненных фильтров документов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SavedFilters + "/All")]
        [ResponseType(typeof(List<FrontDocumentSavedFilter>))]
        public IHttpActionResult Get()
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
            var items = docProc.GetSavedFilters(ctx, new FilterDocumentSavedFilter { IsOnlyCurrentUser = false });
            var res = new JsonResult(items, this);
            return res;
        }

        /// <summary>
        /// Возвращает список сохраненных фильтров документов для текущего пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SavedFilters+"/CurrentUser")]
        [ResponseType(typeof(List<FrontDocumentSavedFilter>))]
        public IHttpActionResult GetOnlyCurrentUser()
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
            var items = docProc.GetSavedFilters(ctx, new FilterDocumentSavedFilter { IsOnlyCurrentUser = true});
            var res = new JsonResult(items, this);
            return res;
        }

        /// <summary>
        /// Возвращает сохраненный фильтр документов по ИД
        /// </summary>
        /// <param name="Id">ИД фильтра</param>
        /// <returns>запись пункта плана</returns>
        [HttpGet]
        [Route(Features.SavedFilters + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentSavedFilter))]
        public IHttpActionResult GetById(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
            var item = docProc.GetSavedFilter(ctx, Id);
            var res = new JsonResult(item, this);
            return res;
        }

        /// <summary>
        /// Добавляет сохраненный фильтр документов плана TODO переделать входящие модели
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.SavedFilters)]
        public IHttpActionResult Post([FromBody]AddDocumentSavedFilter model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.AddSavedFilter, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Измененяет сохраненный фильтр документов
        /// </summary>
        /// <param name="model">Модель для обновления</param>
        /// <returns>Обновленный пункт плана</returns>
        [HttpPut]
        [Route(Features.SavedFilters)]
        public IHttpActionResult Put([FromBody]ModifyDocumentSavedFilter model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.ModifySavedFilter, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Удаляет сохраненный фильтр документов
        /// </summary>
        /// <param name="Id">ИД сохраненного фильтра документов</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.SavedFilters + "/{Id:int}")]
        public IHttpActionResult Delete(int Id)
        {
            Action.Execute(EnumDocumentActions.DeleteSavedFilter, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }
        
    }
}
