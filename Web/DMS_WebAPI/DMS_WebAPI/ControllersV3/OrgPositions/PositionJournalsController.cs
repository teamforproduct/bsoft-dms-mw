using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.Tree;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.OrgPositions
{
    /// <summary>
    /// Должности. Журналы регистрации документов. Устанавливает доступ должности просматривать и регистрировать документы в журналах регистрации
    /// По умолчанию должность может просматривать и регистрировать документы во всех журналах своего отдела 
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Position)]
    public class PositionJournalsController : ApiController
    {
        /// <summary>
        /// Возвращает список журналов регистрации с пычками
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Journals)]
        [ResponseType(typeof(List<TreeItem>))]
        public async Task<IHttpActionResult> Get([FromUri] int Id, [FromUri] FilterTree filter)
        {
            if (filter == null) filter = new FilterTree();
            filter.IsChecked = true;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRegistrationJournalPositionsDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Режим корректировки. Возвращает полный список журналов регистрации с пычками для установки доступа на просмотр или регистрацию
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Journals+ "/Edit")]
        [ResponseType(typeof(List<TreeItem>))]
        public async Task<IHttpActionResult> GetEdit([FromUri] int Id, [FromUri] FilterTree filter)
        {
            if (filter == null) filter = new FilterTree();
            filter.IsChecked = null;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRegistrationJournalPositionsDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности к журналу регистрации на просмотр или на регистрацию
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Journals + "/Set")]
        public async Task<IHttpActionResult> Set([FromBody] SetJournalAccess model)
        {
            var tmpItem = Action.Execute(EnumAdminActions.SetJournalAccess, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности к журналам регистрации в указанном отделе и дочерних
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Journals + "/SetByDepartment")]
        public async Task<IHttpActionResult> SetByDepartment([FromBody] SetJournalAccessByDepartment_Position model)
        {
            var tmpItem = Action.Execute(EnumAdminActions.SetJournalAccessByDepartment_Position, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности к журналам регистрации в для всех отделов указанной компании
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Journals + "/SetByCompany")]
        public async Task<IHttpActionResult> SetByCompany([FromBody] SetJournalAccessByCompany_Position model)
        {
            var tmpItem = Action.Execute(EnumAdminActions.SetJournalAccessByCompany_Position, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }


        /// <summary>
        /// Устанавливает доступ для должности к журналам регистрации в своем отделе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Journals + "/SetDefault")]
        public async Task<IHttpActionResult> SetDefault([FromBody] ModifyAdminDefaultByPosition model)
        {
            var tmpItem = Action.Execute(EnumAdminActions.SetJournalAccessDefault_Position, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности ко всем журналам регистрации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Journals + "/SetAll")]
        public async Task<IHttpActionResult> SetAll([FromBody] SetJournalAccessAll_Position model)
        {
            var tmpItem = Action.Execute(EnumAdminActions.SetJournalAccessAll_Position, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Копирует настроки доступов к журналам регистрации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Journals + "/Duplicate")]
        public async Task<IHttpActionResult> Duplicate([FromBody] CopyAdminSettingsByPosition model)
        {
            var tmpItem = Action.Execute(EnumAdminActions.DuplicateJournalAccess_Position, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
