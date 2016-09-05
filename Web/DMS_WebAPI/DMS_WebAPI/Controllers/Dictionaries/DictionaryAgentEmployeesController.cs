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

namespace DMS_WebAPI.Controllers.Dictionaries
{/// <summary>
 /// Контрагент - сотрудник
 /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/DictionaryAgentEmployees")]
    public class DictionaryAgentEmployeesController : ApiController
    {
        /// <summary>
        /// Список всех сотрудников
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [ResponseType(typeof(List<FrontDictionaryAgentEmployee>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentEmployee filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentEmployees(ctx, filter, paging);
            var res = new JsonResult(tmpDicts, this);
            res.Paging = paging;
            return res;
        }

        /// <summary>
        /// Запись справочника сотрудников
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(FrontDictionaryAgentEmployee))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgentEmployee(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Получение нового табельного номера
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPersonnelNumber")]
        public IHttpActionResult GetPersonnelNumber(bool getNewNumber)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgentEmployeePersonnelNumber(ctx);
            return new JsonResult(tmpDict, this);
        }
        /// <summary>
        /// Добавление сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryAgentEmployee model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentEmployee, ctx, model));
        }

        /// <summary>
        /// сделать физлицо сотрудником
        /// </summary>
        /// <param name="AgentPersonId">ИД агента</param>
        /// <param name="model">параметры сотрудника</param>
        /// <returns>добавленную запись</returns>
        public IHttpActionResult PostToExistingAgent(int AgentPersonId, [FromBody]ModifyDictionaryAgentEmployee model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            model.Id = AgentPersonId;
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentEmployee, ctx, model));
        }

        /// <summary>
        /// Изменение сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgentEmployee model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentEmployee, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentEmployee, ctx, id);
            FrontDictionaryAgentPerson tmp = new FrontDictionaryAgentPerson();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}
