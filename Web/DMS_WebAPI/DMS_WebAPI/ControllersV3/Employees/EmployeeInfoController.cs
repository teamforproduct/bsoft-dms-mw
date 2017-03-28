using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
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
using System.Threading.Tasks;
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
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryAgentEmployee(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

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
        public async Task<IHttpActionResult> GetMain([FromUri]FullTextSearch ftSearch, [FromUri] FilterDictionaryAgentEmployee filter, [FromUri]UIPaging paging, [FromUri]UISorting sorting)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetMainAgentEmployees(context, ftSearch, filter, paging, sorting);
                var res = new JsonResult(tmpItems, this);
                res.Paging = paging;
                return res;
            });
        }


        /// <summary>
        /// Возвращает реквизиты сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontAgentEmployee))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Post([FromBody]AddAgentEmployeeUser model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   var webSeevice = DmsResolver.Current.Get<WebAPIService>();

                   var tmpItem = webSeevice.AddUserEmployee(context, model);

                   return GetById(context, tmpItem);
               });
        }

        /// <summary>
        /// Корректирует реквизиты сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyAgentEmployee model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var webSeevice = DmsResolver.Current.Get<WebAPIService>();
                webSeevice.UpdateUserEmployee(context, model);
                //TODO ASYNC
                var contexts = DmsResolver.Current.Get<UserContexts>();
                contexts.UpdateLanguageId(model.Id, model.LanguageId);

                return GetById(context, model.Id);
            });
        }

        /// <summary>
        /// Удаляет сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var webSeevice = DmsResolver.Current.Get<WebAPIService>();
                webSeevice.DeleteUserEmployee(context, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }



        /// <summary>
        /// Удаляет картинку (аватарку) сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/DeleteImage/{Id:int}")]
        public async Task<IHttpActionResult> DeleteImage([FromUri] int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                Action.Execute(context, EnumDictionaryActions.DeleteAgentImage, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }


    }
}
