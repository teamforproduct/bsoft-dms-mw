using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
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
        Stopwatch stopWatch = new Stopwatch();


        /// <summary>
        /// Типы адресов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.AddressTypes)]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterDictionaryAddressType filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetShortListAddressTypes(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            //res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Типы контактов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.ContactTypes)]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterDictionaryContactType filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetShortListContactTypes(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            //res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Внешние агенты
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Agents)]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetShortListAgentExternal(ctx, paging);
            var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(ctx, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
            var res = new JsonResult(tmpItems, metaData, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Банки
        /// </summary>
        /// <param name="filter">параметры фильтрации</param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Banks)]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterDictionaryAgentBank filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetShortListAgentBanks(ctx, filter, paging);
            var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(ctx, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
            var res = new JsonResult(tmpItems, metaData, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Юридические лица
        /// </summary>
        /// <param name="filter">параметры фильтрации</param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Companies)]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterDictionaryAgentCompany filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentCompanyList(ctx, filter, paging);
            var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(ctx, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
            var res = new JsonResult(tmpItems, metaData, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Отделы
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Departments)]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public IHttpActionResult GetListDepartments([FromUri] FilterDictionaryDepartment filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDepartmentsShortList(ctx, filter);
            var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(ctx, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
            var res = new JsonResult(tmpItems, metaData, this);
            res.SpentTime = stopWatch;
            return res;
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
        public IHttpActionResult GetList([FromUri] FilterDictionaryDocumentType filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetShortListDocumentTypes(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
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
        public IHttpActionResult GetList([FromUri] FilterDictionaryAgentEmployee filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentEmployeeList(ctx, filter, paging);
            var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(ctx, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
            var res = new JsonResult(tmpItems, metaData, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
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
        //public IHttpActionResult GetListDepartments([FromUri] FilterDictionaryPositionExecutor filter, [FromUri]UIPaging paging)
        //{
        //    if (!stopWatch.IsRunning) stopWatch.Restart();
        //    var ctx = DmsResolver.Current.Get<UserContexts>().Get();
        //    var tmpService = DmsResolver.Current.Get<IDictionaryService>();
        //    var tmpItems = tmpService.GetShortListPositionExecutors(ctx, filter, paging);
        //    var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(ctx, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
        //    var res = new JsonResult(tmpItems, metaData, this);
        //    res.SpentTime = stopWatch;
        //    return res;
        //}

        /// <summary>
        /// Журналы
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Journals)]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public IHttpActionResult GetList([FromUri]FilterDictionaryRegistrationJournal filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetRegistrationJournalsShortList(ctx, filter);
            //var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(ctx, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
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
        public IHttpActionResult GetList([FromUri] FilterDictionaryAgentPerson filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetShortListAgentPersons(ctx, filter, paging);
            var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(ctx, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
            var res = new JsonResult(tmpItems, metaData, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Должности, акцент на название должности
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Positions)]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public IHttpActionResult GetList([FromUri]FilterDictionaryPosition filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetPositionsShortList(ctx, filter);
            var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(ctx, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
            var res = new JsonResult(tmpItems, metaData, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Должности, акцент на текущем исполнителе по должности
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Positions+ "/Executor")]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public IHttpActionResult PositionsExecutors([FromUri]FilterDictionaryPosition filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetPositionsExecutorShortList(ctx, filter);
            var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(ctx, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
            var res = new JsonResult(tmpItems, metaData, this);
            res.SpentTime = stopWatch;
            return res;
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
        public IHttpActionResult GetList([FromUri] FilterDictionaryTag filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetTagList(ctx, filter, paging);
            var metaData = new { FavouriteIDs = tmpService.GetFavouriteList(ctx, tmpItems, ApiPrefix.CurrentModule(), ApiPrefix.CurrentFeature()) };
            var res = new JsonResult(tmpItems, metaData, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает массив ИД юзеров, которые онлайн
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.OnlineUsers)]
        [ResponseType(typeof(List<int>))]
        public IHttpActionResult GetOnlineUsers()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var ctx = ctxs.Get();// (keepAlive: false);
            var sesions = ctxs.GetContextListQuery();
            var tmpService = DmsResolver.Current.Get<ILogger>();
            var tmpItems = tmpService.GetOnlineUsers(ctx, sesions);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}