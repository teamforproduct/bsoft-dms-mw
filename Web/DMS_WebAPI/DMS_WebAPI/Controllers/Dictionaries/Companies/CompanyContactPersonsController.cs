using System;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using System.Web;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Text;
using BL.Logic.SystemServices.TempStorage;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Контактные лица компании (юридического лица)
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "Companies")]
    public class CompanyContactPersonsController : ApiController
    {
        /// <summary>
        /// Возвращает список контактных лиц
        /// </summary>
        /// <param name="CompanyId">Компания</param>
        /// <param name="filter">фильтр</param>
        /// <returns>Список контрагентов
        /// </returns>
        [HttpGet]
        [Route("ContactPersons")]
        [ResponseType(typeof(List<FrontContactPersons>))]
        public IHttpActionResult Get(int CompanyId, [FromUri] FilterDictionaryAgentPerson filter)
        {
            if (filter == null) filter = new FilterDictionaryAgentPerson();

            if (filter.AgentCompanyIDs == null) filter.AgentCompanyIDs = new List<int> { CompanyId };
            else filter.AgentCompanyIDs.Add(CompanyId);

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetContactPersons(ctx, filter);
            var res=new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает контактное лицо
        /// </summary>
        /// <param name="Id">ИД агента</param>
        /// <returns>Агент</returns>
        [HttpGet]
        [Route("ContactPersons/{Id:int}")]
        [ResponseType(typeof(FrontContactPersons))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetContactPerson(ctx, Id);
            return new JsonResult(tmpItem, this);
        }
        /// <summary>
        /// Добавляет контактное лицо
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ContactPersons")]
        public IHttpActionResult Post([FromBody]AddAgentContactPerson model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = (int)tmpService.ExecuteAction(EnumDictionaryActions.AddContactPerson, ctx, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует контактное лицо
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("ContactPersons")]
        public IHttpActionResult Put([FromBody]ModifyAgentContactPerson model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = (int)tmpService.ExecuteAction(EnumDictionaryActions.ModifyContactPerson, ctx, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Удаляет контактное лицо
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("ContactPersons/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            tmpService.ExecuteAction(EnumDictionaryActions.DeleteContactPerson, ctx, id);
            FrontDictionaryAgent tmp = new FrontDictionaryAgent();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }

        /// <summary>
        /// Добавляет контактное лицо из существующих физ.лиц
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ContactPersons/AddFromPersons")]
        public IHttpActionResult AddFromExists([FromBody]LinkDictionaryAgentContactPerson model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = (int)tmpService.ExecuteAction(EnumDictionaryActions.AddContactPerson, ctx, model);
            return Get(tmpItem);
        }

    }
}