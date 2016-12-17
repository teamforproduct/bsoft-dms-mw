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
using BL.Model.DictionaryCore.FrontModel;
using System.Web.Http.Description;
using System.Diagnostics;
using BL.Model.Common;

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Описывает список сотрудников, которые являются администраторами подразделения.
    /// </summary>
    [Authorize]
    public class AdminDepartmentAdminsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список имен сотрудников-администраторов для подразделения
        /// </summary>
        /// <param name="DepartmentId">Id подразделения</param>
        /// <returns></returns>
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult Get(int DepartmentId)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetDepartmentAdmins(ctx, DepartmentId);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет сотрудника на роль администратора подразделения
        /// </summary>
        /// <param name="model">ModifyAdminUserRole</param>
        /// <returns>FrontAdminUserRole</returns>
        public IHttpActionResult Post([FromBody]ModifyAdminDepartmentAdmin model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.AddDepartmentAdmin, cxt, model);
            return Get((int)tmpItem);
        }

        /// <summary>
        /// Удаляет сотрудника с роли администратора подразделения
        /// </summary>
        /// <returns>FrontAdminUserRole</returns> 
        public IHttpActionResult Delete([FromUri]ModifyAdminDepartmentAdmin model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            tmpService.ExecuteAction(EnumAdminActions.DeleteDepartmentAdmin, cxt, model);
            var tmpItem = new ListItem() { Id = model.EmployeeId };
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}