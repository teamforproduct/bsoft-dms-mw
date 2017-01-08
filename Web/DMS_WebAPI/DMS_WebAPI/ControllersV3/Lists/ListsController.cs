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

namespace DMS_WebAPI.ControllersV3.Lists
{
    /// <summary>
    /// Списки Id, Name. Для вызова апи требуется авторизация (доступ для авторизованных пользователей не ограничивается).
    /// Подразумевается использование этих апи в выпадающих списках при корректировках ссылочных полей сущности.
    /// В списках отображаются только активные элементы справочников.
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.List)]
    public class ListsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Юридические лица
        /// </summary>
        /// <param name="filter">параметры фильтрации</param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Companies")]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterDictionaryAgentCompany filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentCompanyList(ctx, filter, paging);
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
        [Route("Employees")]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterDictionaryAgentEmployee filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentEmployeeList(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Отделы
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Departments")]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetListDepartments([FromUri] FilterTree filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDepartmentShortList(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Должности
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Positions")]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterDictionaryPosition filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetPositionList(ctx, filter, paging); 
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        

        /// <summary>
        /// Журналы
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Journals")]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterTree filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetRegistrationJournalShortList(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Типы адресов
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AddressTypes")]
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
        [Route("ContactTypes")]
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
        /// Теги
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Tags")]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterDictionaryTag filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetTagList(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }
    }
}