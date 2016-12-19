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

namespace DMS_WebAPI.ControllersV3.Lists
{
    /// <summary>
    /// Списки Id, Name. Для вызова апи требуется авторизация (доступ для авторизованных пользователей не ограничивается).
    /// Подразумевается использование этих апи в выпадающих списках при корректировках ссылочных полей сущности.
    /// В списках отображаются только активные элементы справочников.
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + "List")]
    public class ListsController : ApiController
    {

        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Юридические лица
        /// </summary>
        /// <param name="EmployeeId">ИД агента</param>
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

    }
}