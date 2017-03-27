using BL.CrossCutting.DependencyInjection;
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
    public class CompanyContactPersonsController : ApiController
    {
        /// <summary>
        /// Возвращает список контактных лиц
        /// </summary>
        /// <param name="Id">Компания</param>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.ContactPersons)]
        [ResponseType(typeof(List<FrontContactPersons>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterDictionaryAgentPerson filter)
        {
            if (filter == null) filter = new FilterDictionaryAgentPerson();
            filter.CompanyIDs = new List<int> { Id };

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentPersonsWithContacts(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает контактное лицо
        /// </summary>
        /// <param name="Id">ИД агента</param>
        /// <returns>Агент</returns>
        [HttpGet]
        [Route(Features.ContactPersons + "/{Id:int}")]
        [ResponseType(typeof(FrontAgentPerson))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentPerson(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }
        /// <summary>
        /// Добавляет контактное лицо
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.ContactPersons)]
        public IHttpActionResult Post([FromBody]AddAgentPerson model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddAgentPerson, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует контактное лицо
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.ContactPersons)]
        public IHttpActionResult Put([FromBody]ModifyAgentPerson model)
        {
            Action.Execute(EnumDictionaryActions.ModifyAgentPerson, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет контактное лицо
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.ContactPersons + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteAgentPerson, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;

        }

    }
}