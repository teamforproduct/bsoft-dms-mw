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
using BL.Model.Tree;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Journals
{
    /// <summary>
    /// Журналы регистрации.
    /// Документы всегда регистрируются в журнале.
    /// Журнал регистрации диктует номер нового документа.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Journal)]
    public class JournalInfoController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetRegistrationJournal(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список журналов сгруппированных по отделам и компаниям (дерево Компании-Отделы-Журналы). 
        /// </summary>
        /// <param name="searchInJournals">Выполнять поиск в журналах</param>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Filters")]
        [ResponseType(typeof(List<TreeItem>))]
        public async Task<IHttpActionResult> Get([FromUri]FullTextSearch ftSearch, [FromUri] FilterDictionaryJournalsTree filter, [FromUri]bool? searchInJournals = false)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetRegistrationJournalsFilter(context, searchInJournals ?? false, ftSearch, filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает список журналов. 
        /// </summary>
        /// <param name="ftSearch">"</param>
        /// <param name="filter">"</param>
        /// <param name="paging">"</param>
        /// <param name="sorting">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontDictionaryRegistrationJournal>))]
        public async Task<IHttpActionResult> GetMain([FromUri]FullTextSearch ftSearch, [FromUri] FilterDictionaryRegistrationJournal filter, [FromUri]UIPaging paging, [FromUri]UISorting sorting)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetMainRegistrationJournals(context, ftSearch, filter, paging, sorting);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает список журналов. 
        /// </summary>
        /// <param name="filter">"</param>
        /// <param name="paging">"</param>
        /// <param name="sorting">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info)]
        [ResponseType(typeof(List<FrontDictionaryRegistrationJournal>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionaryRegistrationJournal filter, [FromUri]UIPaging paging, [FromUri]UISorting sorting)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetRegistrationJournals(context, filter, paging, sorting);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает журнал регистрации по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryRegistrationJournal))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет журнал регистрации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Post([FromBody]AddRegistrationJournal model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDictionaryActions.AddRegistrationJournal, model);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует реквизиты журнала регистрации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyRegistrationJournal model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDictionaryActions.ModifyRegistrationJournal, model);
                return GetById(context, model.Id);
            });
        }

        /// <summary>
        /// Удаляет журнал регистрации
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDictionaryActions.DeleteRegistrationJournal, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

    }
}
