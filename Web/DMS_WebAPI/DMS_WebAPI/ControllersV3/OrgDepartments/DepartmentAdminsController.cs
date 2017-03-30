using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetDepartmentAdmins(context, Id);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает список администраторов (сотрудников)
        /// </summary>
        /// <param name="Id">ИД отдела</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Admins)]
        [ResponseType(typeof(List<FrontAdminEmployeeDepartments>))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Создает сотрудника на роль администратора подразделения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Admins)]
        public async Task<IHttpActionResult> Post([FromBody]AddAdminDepartmentAdmin model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumAdminActions.AddDepartmentAdmin, model);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Удаляет сотрудника с роли администратора подразделения
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Admins + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumAdminActions.DeleteDepartmentAdmin, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }
    }
}