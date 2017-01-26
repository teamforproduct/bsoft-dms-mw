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
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// !!! Доступ не ограничен.
    /// Контакты пользователя-сотрудника
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserContactsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список контактов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Contacts)]
        [ResponseType(typeof(List<FrontDictionaryAgentContact>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryContact filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();

            if (filter == null) filter = new FilterDictionaryContact();
            filter.AgentIDs = new List<int> { ctx.CurrentAgentId };

            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentContacts(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает контакт по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Contacts + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryAgentContact))]
        public IHttpActionResult Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentContact(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Создает новый контакт
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Contacts)]
        public IHttpActionResult Post([FromBody]BaseAgentContact model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var contact = new AddAgentContact(model);
            contact.AgentId = ctx.CurrentAgentId;
            var tmpItem = Action.Execute(EnumDictionaryActions.AddEmployeeContact, contact);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует контакт
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Contacts)]
        public IHttpActionResult Put([FromBody]ModifyUserContact model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var contact = new ModifyAgentContact(model);
            contact.AgentId = ctx.CurrentAgentId;
            Action.Execute(EnumDictionaryActions.ModifyEmployeeContact, contact);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет контакт
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Contacts + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDictionaryActions.DeleteEmployeeContact, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}