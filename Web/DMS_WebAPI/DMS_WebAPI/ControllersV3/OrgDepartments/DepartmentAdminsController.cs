using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.OrgDepartments
{
    /// <summary>
    /// Отделы органицации. Локальные администраторы
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Department)]
    public class DepartmentAdminsController : ApiController
    {
        /// <summary>
        /// Возвращает список администраторов (сотрудников)
        /// </summary>
        /// <param name="Id">ИД отдела</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Admins)]
        [ResponseType(typeof(List<FrontAdminEmployeeDepartments>))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetDepartmentAdmins(ctx, Id);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Создает сотрудника на роль администратора подразделения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Admins)]
        public IHttpActionResult Post([FromBody]AddAdminDepartmentAdmin model)
        {
            var tmpItem = Action.Execute(EnumAdminActions.AddDepartmentAdmin, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Удаляет сотрудника с роли администратора подразделения
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Admins + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumAdminActions.DeleteDepartmentAdmin, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }
    }
}