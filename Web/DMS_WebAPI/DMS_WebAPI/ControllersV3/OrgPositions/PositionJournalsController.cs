using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.SystemCore;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;

using BL.Model.Common;
using System.Web;
using BL.Logic.SystemServices.TempStorage;
using BL.Model.DictionaryCore.FrontMainModel;
using System.Diagnostics;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.FilterModel;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Tree;

namespace DMS_WebAPI.ControllersV3.OrgPositions
{
    /// <summary>
    /// Журналы регистрации документов. Устанавливает доступ должности просматривать и регистрировать документы в журналах регистрации
    /// По умолчанию должность может просматривать и регистрировать документы во всех журналах своего отдела 
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Position)]
    public class PositionJournalsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список журналов регистрации с пычками
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/Journals")]
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult Get([FromUri] int Id, [FromUri] FilterTree filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterTree();
            filter.IsChecked = true;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRegistrationJournalPositionsDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Режим корректировки. Возвращает полный список журналов регистрации с пычками для установки доступа на просмотр или регистрацию
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/Journals/Edit")]
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult GetEdit([FromUri] int Id, [FromUri] FilterTree filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterTree();
            filter.IsChecked = false;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRegistrationJournalPositionsDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности к журналу регистрации на просмотр или на регистрацию
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Journals/Set")]
        public IHttpActionResult Set([FromBody] ModifyAdminRegistrationJournalPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetRegistrationJournalPosition, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности к журналам регистрации в указанном отделе и дочерних
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Journals/SetByDepartment")]
        public IHttpActionResult SetByDepartment([FromBody] ModifyAdminRegistrationJournalPositionByDepartment model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetRegistrationJournalPositionByDepartment, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности к журналам регистрации в для всех отделов указанной компании
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Journals/SetByCompany")]
        public IHttpActionResult SetByCompany([FromBody] ModifyAdminRegistrationJournalPositionByCompany model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetRegistrationJournalPositionByCompany, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Устанавливает доступ для должности к журналам регистрации в своем отделе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Journals/SetDefault")]
        public IHttpActionResult SetDefault([FromBody] ModifyAdminDefaultByPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetDefaultRegistrationJournalPosition, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности ко всем журналам регистрации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Journals/SetAll")]
        public IHttpActionResult SetAll([FromBody] ModifyAdminRegistrationJournalPositions model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetAllRegistrationJournalPosition, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Копирует настроки доступов к журналам регистрации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Journals/Duplicate")]
        public IHttpActionResult Duplicate([FromBody] CopyAdminSettingsByPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.DuplicateRegistrationJournalPositions, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
