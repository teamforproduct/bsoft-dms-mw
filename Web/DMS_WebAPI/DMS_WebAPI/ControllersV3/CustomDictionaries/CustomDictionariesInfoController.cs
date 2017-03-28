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
    /// Пользовательские справочники. Типы
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.CustomDictionaries)]
    public class CustomDictionariesInfoController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetCustomDictionaryType(context, Id);
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
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontCustomDictionary>))]
        public async Task<IHttpActionResult> GetMain(int Id, [FromUri]FullTextSearch ftSearch, [FromUri]FilterCustomDictionaryType filter, [FromUri]UIPaging paging, [FromUri]UISorting sorting)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                   var tmpItems = tmpService.GetMainCustomDictionaryTypes(context, ftSearch, filter, paging, sorting);
                   var res = new JsonResult(tmpItems, this);
                   res.Paging = paging;
                   return res;
               });
        }

        /// <summary>
        /// Возвращает список пользовательских справочников
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info)]
        [ResponseType(typeof(List<FrontCustomDictionaryType>))]
        public async Task<IHttpActionResult> Get([FromUri]FilterCustomDictionaryType filter)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                   var tmpItem = tmpService.GetCustomDictionaryTypes(context, filter);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Возвращает реквизиты пользовательского справочника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontCustomDictionaryType))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет пользовательский справочник
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Post([FromBody]AddCustomDictionaryType model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpItem = Action.Execute(context, EnumDictionaryActions.AddCustomDictionaryType, model);
                   return GetById(context, tmpItem);
               });
        }

        /// <summary>
        /// Корректирует реквизиты пользовательского справочника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyCustomDictionaryType model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDictionaryActions.ModifyCustomDictionaryType, model);
                   return GetById(context, model.Id);
               });
        }

        /// <summary>
        /// Удаляет пользовательский справочник
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDictionaryActions.DeleteCustomDictionaryType, Id);
                   var tmpItem = new FrontDeleteModel(Id);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

    }
}
