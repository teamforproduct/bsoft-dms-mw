using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Companies
{
    /// <summary>
    /// Контактные лица компании (юридического лица)
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Company)]
    public class CompanyContactPersonsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список контактных лиц
        /// </summary>
        /// <param name="CompanyId">Компания</param>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ContactPersons")]
        [ResponseType(typeof(List<FrontContactPersons>))]
        public IHttpActionResult Get(int CompanyId, [FromUri] FilterDictionaryAgentPerson filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterDictionaryAgentPerson();

            if (filter.AgentCompanyIDs == null) filter.AgentCompanyIDs = new List<int> { CompanyId };
            else filter.AgentCompanyIDs.Add(CompanyId);

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetContactPersons(ctx, filter);
            var res=new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
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
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetContactPerson(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
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
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDictionaryActions.AddContactPerson, model);
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
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDictionaryActions.ModifyContactPerson, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет контактное лицо
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("ContactPersons/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDictionaryActions.DeleteContactPerson, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;

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