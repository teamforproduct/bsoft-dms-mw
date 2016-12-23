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

namespace DMS_WebAPI.ControllersV3.Employees
{
    /// <summary>
    /// Сотрудник
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Employee)]
    public class EmployeeInfoController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Список сотрудников
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Info/Main")]
        [ResponseType(typeof(List<FrontListAgentEmployee>))]
        public IHttpActionResult GetWithPositions([FromUri] FilterDictionaryAgentEmployee filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryAgentEmployees(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Возвращает реквизиты сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Info/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryAgentEmployee))]
        public IHttpActionResult Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryAgentEmployee(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Info")]
        public IHttpActionResult Post([FromBody]AddAgentEmployeeUser model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var webSeevice = new WebAPIService();

            var tmpItem = webSeevice.AddUserEmployee(ctx, model);

            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует реквизиты сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Info")]
        public IHttpActionResult Put([FromBody]ModifyAgentEmployee model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var contexts = DmsResolver.Current.Get<UserContexts>();
            var ctx = contexts.Get();
            var webSeevice = new WebAPIService();

            webSeevice.UpdateUserEmployee(ctx, model);

            contexts.UpdateLanguageId(model.Id, model.LanguageId);

            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Info/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var webSeevice = new WebAPIService();
            webSeevice.DeleteUserEmployee(ctx, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
