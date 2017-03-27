using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.OrgPositions
{
    /// <summary>
    /// Должности. Роли (обязанности), которые возлагаются на должность. 
    /// Когда на должность назначается исполнитель, ио или референт, система задает вопрос: "Какие роли из всех возложенных на должность может выполнять сотрудник. Референт может не иметь права подписания и тд..."
    /// При изменнии ролей для должности возникает задача синхронизации шаблона и экземпляров
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Position)]
    public class PositionRolesController : ApiController
    {
        /// <summary>
        /// Возвращает роли должности
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Roles)]
        [ResponseType(typeof(List<FrontAdminPositionRole>))]
        public async Task<IHttpActionResult> Get([FromUri] int Id, [FromUri] FilterAdminPositionRoleDIP filter)
        {
            if (filter == null) filter = new FilterAdminPositionRoleDIP();
            filter.PositionIDs = new List<int> { Id };
            filter.IsChecked = true;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetPositionRolesDIP(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Режим корректировки. Возвращает роли должности
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Roles + "/Edit")]
        [ResponseType(typeof(List<FrontAdminPositionRole>))]
        public async Task<IHttpActionResult> GetEdit([FromUri] int Id, [FromUri] FilterAdminPositionRoleDIP filter)
        {
            if (filter == null) filter = new FilterAdminPositionRoleDIP();
            filter.PositionIDs = new List<int> { Id };
            filter.IsChecked = null;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetPositionRolesDIP(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Добавляет/удаляет роль должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Roles + "/Set")]
        public async Task<IHttpActionResult> Set([FromBody] SetAdminPositionRole model)
        {
            var tmpItem = Action.Execute(EnumAdminActions.SetPositionRole, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }


        /// <summary>
        /// Копирует роли
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Roles + "/Duplicate")]
        public async Task<IHttpActionResult> Duplicate([FromBody] CopyAdminSettingsByPosition model)
        {
            var tmpItem = Action.Execute(EnumAdminActions.DuplicatePositionRoles, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
