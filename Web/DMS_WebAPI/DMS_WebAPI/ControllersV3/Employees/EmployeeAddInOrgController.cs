using BL.CrossCutting.DependencyInjection;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.Employees
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Employee)]
    public class EmployeeAddInOrgController : WebApiController
    {

        /// <summary>
        /// Добавляет сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.AddInOrg)]
        public IHttpActionResult Post([FromBody]AddEmployeeInOrg model)
        {
            //!SYNC
            var context = DmsResolver.Current.Get<UserContexts>().Get();

            var webSeevice = DmsResolver.Current.Get<WebAPIService>();

            var tmpItem = webSeevice.AddUserEmployeeInOrg(context, model);

            return new JsonResult(tmpItem, this); ;
        }


    }
}
