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
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Roles
{
    /// <summary>
    /// Роли (обязанности), которые возлагаются на должность. 
    /// Когда на должность назначается исполнитель, ио или референт, система задает вопрос: "Какие роли из всех возложенных на должность может выполнять сотрудник. Референт может не иметь права подписания и тд..."
    /// При изменнии ролей для должности возникает задача синхронизации шаблона и экземпляров
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Role)]
    public class RolePermissionsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает действия роли
        /// </summary>
        /// <param name="Id"></param>
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
            var tmpItems = tmpService.GetRolePermissions(ctx, new FilterAdminRolePermissionsDIP { RoleId = Id, IsChecked = true });
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Режим корректировки. Возвращает действия роли
        /// </summary>
        /// <param name="Id"></param>
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
            var tmpItems = tmpService.GetRolePermissions(ctx, new FilterAdminRolePermissionsDIP { RoleId = Id, IsChecked = false });
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

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRolePermissions(ctx, new FilterAdminRolePermissionsDIP {
                RoleId = model.RoleId,
                IsChecked = false,
                Module = model.Module,
                Feature = model.Feature,
            }).FirstOrDefault();

            var res = new JsonResult(tmpItems?.Features.FirstOrDefault(), this);
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

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRolePermissions(ctx, new FilterAdminRolePermissionsDIP
            {
                RoleId = model.RoleId,
                IsChecked = false,
                Module = model.Module,
            }).FirstOrDefault();

            var res = new JsonResult(tmpItems, this);
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

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRolePermissions(ctx, new FilterAdminRolePermissionsDIP
            {
                RoleId = model.RoleId,
                IsChecked = false,
                Module = model.Module,
            }).FirstOrDefault();

            var res = new JsonResult(tmpItems, this);
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

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRolePermissions(ctx, new FilterAdminRolePermissionsDIP
            {
                RoleId = model.RoleId,
                IsChecked = false,
                Module = model.Module,
            });

            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }



    }
}
