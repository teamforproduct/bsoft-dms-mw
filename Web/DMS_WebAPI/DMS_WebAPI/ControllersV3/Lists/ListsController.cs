using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;
using BL.Model.Common;
using BL.Model.SystemCore;
using System.Diagnostics;
using BL.Model.Tree;
using BL.CrossCutting.Interfaces;
using BL.Model.FullTextSearch;

namespace DMS_WebAPI.ControllersV3.Lists
{
    /// <summary>
    /// !!! Доступ не ограничен.
    /// Списки Id, Name. Для вызова апи требуется авторизация (доступ для авторизованных пользователей не ограничивается).
    /// Подразумевается использование этих апи в выпадающих списках при корректировках ссылочных полей сущности.
    /// В списках отображаются только активные элементы справочников.
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.List)]
    public class ListsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();


        /// <summary>
        /// Типы адресов
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
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
        /// <param name="paging"></param>
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
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Departments)]
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult GetListDepartments([FromUri]FullTextSearch ftSearch, [FromUri] FilterDictionaryDepartment filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDepartmentsShortList(ctx, ftSearch, filter);
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

        /// <summary>
        /// Журналы
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Journals)]
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult GetList([FromUri]FullTextSearch ftSearch, [FromUri]FilterDictionaryRegistrationJournal filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetRegistrationJournalsShortList(ctx, ftSearch, filter);
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
        [ResponseType(typeof(List<FrontShortListPosition>))]
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
        /// Должности
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Positions)]
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult GetList([FromUri]FullTextSearch ftSearch, [FromUri]FilterDictionaryPosition filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetPositionsShortList(ctx, ftSearch, filter);
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