using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Companies
{
    /// <summary>
    /// Юридические лица. Контактные лица
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Company)]
    public class CompanyContactPersonsController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentPerson(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список контактных лиц
        /// </summary>
        /// <param name="Id">Компания</param>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.ContactPersons)]
        [ResponseType(typeof(List<FrontContactPersons>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterDictionaryAgentPerson filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                if (filter == null) filter = new FilterDictionaryAgentPerson();
                filter.CompanyIDs = new List<int> { Id };

                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetAgentPersonsWithContacts(context, filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает контактное лицо
        /// </summary>
        /// <param name="Id">ИД агента</param>
        /// <returns>Агент</returns>
        [HttpGet]
        [Route(Features.ContactPersons + "/{Id:int}")]
        [ResponseType(typeof(FrontAgentPerson))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет контактное лицо
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.ContactPersons)]
        public async Task<IHttpActionResult> Post([FromBody]AddAgentPerson model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDictionaryActions.AddAgentPerson, model);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Добавляет контактное лицо из существующих
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.ContactPersons + "/Existing")]
        public async Task<IHttpActionResult> Post([FromBody]AddAgentPersonExisting model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDictionaryActions.AddAgentPersonExisting, model);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует контактное лицо
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.ContactPersons)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyAgentPerson model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDictionaryActions.ModifyAgentPerson, model);
                return GetById(context, model.Id);
            });
        }

        /// <summary>
        /// Удаляет контактное лицо
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.ContactPersons + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDictionaryActions.DeleteAgentPerson, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });

        }

    }
}