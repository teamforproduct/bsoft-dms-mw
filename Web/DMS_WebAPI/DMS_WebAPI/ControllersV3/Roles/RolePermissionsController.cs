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

namespace DMS_WebAPI.ControllersV3.OrgPositions
{
    /// <summary>
    /// Роли (обязанности), которые возлагаются на должность. 
    /// Когда на должность назначается исполнитель, ио или референт, система задает вопрос: "Какие роли из всех возложенных на должность может выполнять сотрудник. Референт может не иметь права подписания и тд..."
    /// При изменнии ролей для должности возникает задача синхронизации шаблона и экземпляров
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Role)]
    public class RolePermissionsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает действия роли
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Permissions)]
        [ResponseType(typeof(List<FrontModule>))]
        public IHttpActionResult Get([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            //if (filter == null) filter = new FilterAdminRole();
            //filter.PositionIDs = new List<int> { Id };
            //filter.IsChecked = true;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRolePermissions(ctx, Id, true);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Режим корректировки. Возвращает действия роли
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Permissions + "/Edit")]
        [ResponseType(typeof(List<FrontModule>))]
        public IHttpActionResult GetEdit([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            //if (filter == null) filter = new FilterAdminRole();
            //filter.PositionIDs = new List<int> { Id };
            //filter.IsChecked = false;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRolePermissions(ctx, Id, false);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет/удаляет действия роли
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Permissions + "/Set")]
        public IHttpActionResult Set([FromBody] SetRolePermission model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetRolePermission, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет/удаляет действия роли, которые принадлежат фичи
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Permissions + "/SetByAccessType")]
        public IHttpActionResult Set([FromBody] SetRolePermissionByModuleAccessType model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetRolePermissionByModuleAccessType, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет/удаляет действия роли, которые принадлежат фичи
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Permissions + "/SetByFeature")]
        public IHttpActionResult Set([FromBody] SetRolePermissionByModuleFeature model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetRolePermissionByModuleFeature, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет/удаляет действия роли, которые принадлежат модулю
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Permissions + "/SetByModule")]
        public IHttpActionResult Set([FromBody] SetRolePermissionByModule model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetRolePermissionByModule, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }



    }
}
