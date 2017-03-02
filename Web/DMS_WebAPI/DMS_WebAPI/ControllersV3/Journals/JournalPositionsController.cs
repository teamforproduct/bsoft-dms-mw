using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.Tree;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Journals
{
    /// <summary>
    /// Журналы регистрации. Доступ.
    /// Управление доступом должностей к журналам регистрации документов.
    /// Доступ бывает двух типов: для просмотра и для регистрации.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Journal)]
    public class JournalPositionsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список должностей, которые имеют доступ к журналу
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Positions + "/Edit")]
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult Get([FromUri] int Id, [FromUri] FilterTree filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterTree();
            filter.IsChecked = false;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetPositionsByJournalDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Режим корректировки. Возвращает полный список должностей для управления доступом на просмотр и на регистрацию.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Positions)]
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult GetEdit([FromUri] int Id, [FromUri] FilterTree filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterTree();
            filter.IsChecked = true;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetPositionsByJournalDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Разрешает/запрещает должности доступ к журналу с учетом типа доступа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Positions + "/Set")]
        public IHttpActionResult Set([FromBody] SetJournalAccess model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetJournalAccess, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Разрешает/запрещает должности доступ к журналам отдела и дочерних отделов с учетом типа доступа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Positions + "/SetByDepartment")]
        public IHttpActionResult SetByDepartment([FromBody] SetJournalAccessByDepartment_Journal model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetJournalAccessByDepartment_Journal, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Разрешает/запрещает должности доступ  для всех журналов организации учетом типа доступа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Positions + "/SetByCompany")]
        public IHttpActionResult SetByCompany([FromBody] SetJournalAccessByCompany_Journal model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetJournalAccessByCompany_Journal, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }


        ///// <summary>
        ///// Устанавливает доступ по умолчанию
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route(Features.Positions + "/SetDefault")]
        //public IHttpActionResult SetDefault([FromBody]SetJournalAccessDefault_Journal model)
        //{
        //    if (!stopWatch.IsRunning) stopWatch.Restart();
        //    var tmpItem = Action.Execute(EnumAdminActions.SetJournalAccessDefault_Journal, model);
        //    var res = new JsonResult(tmpItem, this);
        //    res.SpentTime = stopWatch;
        //    return res;
        //}

        /// <summary>
        /// Разрешает/запрещает должности выполнять рассылку для всех должностей организации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Positions + "/SetAll")]
        public IHttpActionResult SetAll([FromBody]SetJournalAccessAll_Journal model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetJournalAccessAll_Journal, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Копирует настроки рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Positions + "/Duplicate")]
        public IHttpActionResult Duplicate([FromBody] DuplicateJournalAccess model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.DuplicateJournalAccess_Journal, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}
