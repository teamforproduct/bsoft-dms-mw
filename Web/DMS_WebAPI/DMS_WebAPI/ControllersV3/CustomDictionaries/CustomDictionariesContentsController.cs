using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.CustomDictionaries
{
    /// <summary>
    /// Пользовательские справочники. Элементы
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.CustomDictionaries)]
    public class CustomDictionariesContentsController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetCustomDictionary(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список элементов пользовательского справочника
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <param name="sorting">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Contents + "/Main")]
        [ResponseType(typeof(List<FrontCustomDictionary>))]
        public IHttpActionResult GetMain(int Id, [FromUri]FullTextSearch ftSearch, [FromUri]FilterCustomDictionary filter, [FromUri]UIPaging paging, [FromUri]UISorting sorting)
        {
            if (filter == null) filter = new FilterCustomDictionary();
            filter.TypeIDs = new List<int> { Id };

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetMainCustomDictionaries(ctx, ftSearch, filter, paging, sorting);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            return res;
        }


        /// <summary>
        /// Возвращает элемент пользовательского справочника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Contents + "/{Id:int}")]
        [ResponseType(typeof(FrontCustomDictionary))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет элемент пользовательского справочника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Contents)]
        public IHttpActionResult Post([FromBody]AddCustomDictionary model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddCustomDictionary, model);
            return GetById(context, tmpItem);
        }

        /// <summary>
        /// Корректирует элемент пользовательского справочника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Contents)]
        public IHttpActionResult Put([FromBody]ModifyCustomDictionary model)
        {
            Action.Execute(EnumDictionaryActions.ModifyCustomDictionary, model);
            return GetById(context, model.Id);
        }

        /// <summary>
        /// Удаляет элемент пользовательского справочника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Contents + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteCustomDictionary, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
