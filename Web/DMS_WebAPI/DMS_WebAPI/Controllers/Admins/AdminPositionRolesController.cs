using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;
using System.Web.Http.Description;
using System.Diagnostics;

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Структура описывает роли, которые доступны должности. 
    /// Структуру нужно воспринимать как набор шаблонов. 
    /// Когда на должность назначается исполнитель, ио или референт, система задает вопрос: "Какие роли из шаблона может выполнять сотрудник. Референт может не иметь права подписания и тд..."
    /// При изменнии ролей для должности возникает задача синхронизации шаблона и экземпляров
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "AdminPositionRoles")]
    public class AdminPositionRolesController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает роли должностей
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [ResponseType(typeof(List<FrontAdminPositionRole>))]
        public IHttpActionResult Get([FromUri] int positionId, [FromUri] FilterAdminRole filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter.PositionIDs == null) filter.PositionIDs = new List<int>();

            filter.PositionIDs.Add(positionId);

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetPositionRoles(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// GetAdminPositionRoles by ID 
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>FrontAdminPositionRole</returns>
        [ResponseType(typeof(FrontAdminPositionRole))]
        public IHttpActionResult Get(int id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetPositionRole(ctx, id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет роль для должности
        /// </summary>
        /// <param name="model">ModifyAdminPositionRole</param>
        /// <returns>FrontAdminPositionRole</returns>
        [ResponseType(typeof(FrontAdminPositionRole))]
        public IHttpActionResult Post([FromBody]SetAdminPositionRole model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetPositionRole, cxt, model);
            return Get((int)tmpItem);
        }

        /// <summary>
        /// Изменяет роль для должности
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <param name="model">ModifyAdminPositionRole</param>
        /// <returns>FrontAdminPositionRole</returns>
        //[ResponseType(typeof(FrontAdminPositionRole))]
        //public IHttpActionResult Put(int id, [FromBody]ModifyAdminPositionRole model)
        //{
        //    model.Id = id;
        //    var cxt = DmsResolver.Current.Get<UserContext>().Get();
        //    var tmpService = DmsResolver.Current.Get<IAdminService>();
        //    tmpService.ExecuteAction(EnumAdminActions.ModifyPositionRole, cxt, model);
        //    return Get(model.Id);
        //}

        /// <summary>
        /// Удаляет роль для должности
        /// </summary>
        /// <returns>FrontAdminPositionRole</returns> 
        public IHttpActionResult Delete([FromUri] int positionId, [FromUri] int roleId)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            //tmpService.ExecuteAction(EnumAdminActions.DeletePositionRole, cxt, new ModifyAdminPositionRole() { PositionId = positionId, RoleId = roleId });
            var tmpItem = new FrontAdminPositionRole() { PositionId = positionId, RoleId = roleId };
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Копирует роли от одной должности к другой
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Duplicate")]
        public IHttpActionResult Duplicate([FromBody] CopyAdminSettingsByPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.DuplicatePositionRoles, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}