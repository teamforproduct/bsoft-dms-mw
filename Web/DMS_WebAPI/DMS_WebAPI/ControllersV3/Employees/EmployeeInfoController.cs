using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontMainModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Employees
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Employee)]
    public class EmployeeInfoController : ApiController
    {
        /// <summary>
        /// Список сотрудников
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <param name="sorting">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontMainAgentEmployee>))]
        public IHttpActionResult GetMain([FromUri]FullTextSearch ftSearch, [FromUri] FilterDictionaryAgentEmployee filter, [FromUri]UIPaging paging, [FromUri]UISorting sorting)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetMainAgentEmployees(ctx, ftSearch, filter, paging, sorting);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            return res;
        }


        /// <summary>
        /// Возвращает реквизиты сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontAgentEmployee))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryAgentEmployee(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Добавляет сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddAgentEmployeeUser model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var webSeevice = DmsResolver.Current.Get<WebAPIService>();

            var tmpItem = webSeevice.AddUserEmployee(ctx, model);

            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует реквизиты сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyAgentEmployee model)
        {
            var contexts = DmsResolver.Current.Get<UserContexts>();
            var ctx = contexts.Get();
            var webSeevice = DmsResolver.Current.Get<WebAPIService>();

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
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var webSeevice = DmsResolver.Current.Get<WebAPIService>();
            webSeevice.DeleteUserEmployee(ctx, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }



        /// <summary>
        /// Удаляет картинку (аватарку) сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/DeleteImage/{Id:int}")]
        public IHttpActionResult DeleteImage([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteAgentImage, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }


    }
}
