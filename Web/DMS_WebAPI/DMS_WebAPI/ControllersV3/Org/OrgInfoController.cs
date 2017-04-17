using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using BL.Model.Tree;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Org
{
    /// <summary>
    /// Организации - клиентские компании.
    /// Организация - рутовый элемент в иерархии штатного расписания
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Org)]
    public class OrgInfoController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryAgentClientCompany(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает штатное расписание. Компании -> Отделы -> Должности -> Исполнители
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<TreeItem>))]
        public async Task<IHttpActionResult> Get([FromUri]FullTextSearch ftSearch, [FromUri] FilterDictionaryStaffList filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetStaffList(context, ftSearch, filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает список организаций
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info)]
        [ResponseType(typeof(FrontDictionaryAgentClientCompany))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionaryAgentOrg filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItem = tmpService.GetDictionaryAgentClientCompanies(context, filter);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает реквизиты организации
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryAgentClientCompany))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет организацию
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Post([FromBody]AddOrg model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDictionaryActions.AddOrg, model);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует реквизиты организации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyOrg model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDictionaryActions.ModifyOrg, model);
                return GetById(context, model.Id);
            });
        }

        /// <summary>
        /// Удаляет организацию
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDictionaryActions.DeleteOrg, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

    }
}
