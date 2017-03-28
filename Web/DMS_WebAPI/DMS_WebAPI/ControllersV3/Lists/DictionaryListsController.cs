using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Lists
{
    /// <summary>
    /// Списки Id, Name.
    /// Подразумевается использование этих апи в выпадающих списках при корректировках ссылочных полей сущности.
    /// В списках отображаются только активные элементы справочников.
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.List)]
    public class DictionaryListsController : ApiController
    {
        /// <summary>
        /// Типы адресов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.AddressTypes)]
        [ResponseType(typeof(List<ListItem>))]
        public async Task<IHttpActionResult> GetList([FromUri] FilterDictionaryAddressType filter)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetShortListAddressTypes(context, filter);
                var res = new JsonResult(tmpItems, this);
                //res.Paging = paging;
                return res;
            });
        }

        /// <summary>
        /// Типы контактов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.ContactTypes)]
        [ResponseType(typeof(List<ListItem>))]
        public async Task<IHttpActionResult> GetList([FromUri] FilterDictionaryContactType filter)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetShortListContactTypes(context, filter);
                var res = new JsonResult(tmpItems, this);
                //res.Paging = paging;
                return res;
            });
        }

        /// <summary>
        /// Внешние агенты
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Agents)]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public async Task<IHttpActionResult> GetList([FromUri]UIPaging paging)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetShortListAgentExternal(context, paging);
                var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(context, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
                var res = new JsonResult(tmpItems, metaData, this);
                res.Paging = paging;
                return res;
            });
        }

        /// <summary>
        /// Банки
        /// </summary>
        /// <param name="filter">параметры фильтрации</param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Banks)]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public async Task<IHttpActionResult> GetList([FromUri] FilterDictionaryAgentBank filter, [FromUri]UIPaging paging)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetShortListAgentBanks(context, filter, paging);
                var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(context, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
                var res = new JsonResult(tmpItems, metaData, this);
                res.Paging = paging;
                return res;
            });
        }

        /// <summary>
        /// Юридические лица
        /// </summary>
        /// <param name="filter">параметры фильтрации</param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Companies)]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public async Task<IHttpActionResult> GetList([FromUri] FilterDictionaryAgentCompany filter, [FromUri]UIPaging paging)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetAgentCompanyList(context, filter, paging);
                var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(context, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
                var res = new JsonResult(tmpItems, metaData, this);
                res.Paging = paging;
                return res;
            });
        }

        /// <summary>
        /// Отделы
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Departments)]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public async Task<IHttpActionResult> GetListDepartments([FromUri] FilterDictionaryDepartment filter)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetDepartmentsShortList(context, filter);
                var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(context, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
                var res = new JsonResult(tmpItems, metaData, this);
                return res;
            });
        }

        /// <summary>
        /// Типы документов
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.DocumentTypes)]
        [ResponseType(typeof(List<ListItem>))]
        public async Task<IHttpActionResult> GetList([FromUri] FilterDictionaryDocumentType filter, [FromUri]UIPaging paging)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetShortListDocumentTypes(context, filter, paging);
                var res = new JsonResult(tmpItems, this);
                res.Paging = paging;
                return res;
            });
        }

        /// <summary>
        /// Сотрудники
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Employees)]
        [ResponseType(typeof(List<ListItem>))]
        public async Task<IHttpActionResult> GetList([FromUri] FilterDictionaryAgentEmployee filter, [FromUri]UIPaging paging)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetAgentEmployeeList(context, filter, paging);
                var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(context, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
                var res = new JsonResult(tmpItems, metaData, this);
                res.Paging = paging;
                return res;
            });
        }

        ///// <summary>
        ///// Исполнители должностей
        ///// </summary>
        ///// <param name="filter"></param>
        ///// <param name="paging"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route(Features.Executors)]
        //[ResponseType(typeof(List<AutocompleteItem>))]
        //public async Task<IHttpActionResult> GetListDepartments([FromUri] FilterDictionaryPositionExecutor filter, [FromUri]UIPaging paging)
        //{
        //    if (!stopWatch.IsRunning) stopWatch.Restart();
        //    return await this.SafeExecuteAsync(ModelState, context => {
        //    var tmpService = DmsResolver.Current.Get<IDictionaryService>();
        //    var tmpItems = tmpService.GetShortListPositionExecutors(context, filter, paging);
        //    var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(context, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
        //    var res = new JsonResult(tmpItems, metaData, this);
        //    res.SpentTime = stopWatch;
        //    return res;});
        //}

        /// <summary>
        /// Журналы
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Journals)]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public async Task<IHttpActionResult> GetList([FromUri]FilterDictionaryRegistrationJournal filter)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetRegistrationJournalsShortList(context, filter);
                //var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(context, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Физические лица
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Persons)]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public async Task<IHttpActionResult> GetList([FromUri] FilterDictionaryAgentPerson filter, [FromUri]UIPaging paging)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetShortListAgentPersons(context, filter, paging);
                var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(context, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
                var res = new JsonResult(tmpItems, metaData, this);
                res.Paging = paging;
                return res;
            });
        }

        /// <summary>
        /// Должности, акцент на название должности
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Positions)]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public async Task<IHttpActionResult> GetList([FromUri]FilterDictionaryPosition filter)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetPositionsShortList(context, filter);
                var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(context, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
                var res = new JsonResult(tmpItems, metaData, this);
                return res;
            });
        }

        /// <summary>
        /// Должности, акцент на текущем исполнителе по должности
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Positions + "/Executor")]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public async Task<IHttpActionResult> PositionsExecutors([FromUri]FilterDictionaryPosition filter)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetPositionsExecutorShortList(context, filter);
                var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(context, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
                var res = new JsonResult(tmpItems, metaData, this);
                return res;
            });
        }

        /// <summary>
        /// Теги
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Tags)]
        [ResponseType(typeof(List<ListItem>))]
        public async Task<IHttpActionResult> GetList([FromUri] FilterDictionaryTag filter, [FromUri]UIPaging paging)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetTagList(context, filter, paging);
                var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(context, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
                var res = new JsonResult(tmpItems, metaData, this);
                res.Paging = paging;
                return res;
            });
        }

        /// <summary>
        /// Возвращает массив ИД юзеров, которые онлайн
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.OnlineUsers)]
        [ResponseType(typeof(List<int>))]
        public async Task<IHttpActionResult> GetOnlineUsers()
        {
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var sesions = ctxs.GetContextListQuery();

            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ILogger>();
                var tmpItems = tmpService.GetOnlineUsers(context, sesions);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }
    }
}