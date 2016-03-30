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

namespace DMS_WebAPI.Controllers.Dictionaries
    {/// <summary>
    /// Контрагент - сотрудник
    /// </summary>
    [Authorize]
    public class DictionaryAgentEmployeesController : ApiController
    {
            /// <summary>
            /// Список всех сотрудников
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            public IHttpActionResult Get([FromUri] FilterDictionaryAgentEmployee filter, [FromUri]UIPaging paging)
            {
                var cxt = DmsResolver.Current.Get<UserContext>().Get();
                var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
                var tmpDicts = tmpDictProc.GetDictionaryAgentEmployees(cxt, filter,paging);
                return new JsonResult(tmpDicts, this);
            }

            /// <summary>
            /// Запись справочника сотрудников
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public IHttpActionResult Get(int id)
            {
                var cxt = DmsResolver.Current.Get<UserContext>().Get();
                var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
                var tmpDict = tmpDictProc.GetDictionaryAgentEmployee(cxt, id);
                return new JsonResult(tmpDict, this);
            }
            /// <summary>
            /// Добавление сотрудника
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public IHttpActionResult Post([FromBody]ModifyDictionaryAgentEmployee model)
            {
                var cxt = DmsResolver.Current.Get<UserContext>().Get();
                var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
                return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentEmployee, cxt, model));
            }

        /// <summary>
        /// сделать физлицо сотрудником
        /// </summary>
        /// <param name="AgentPersonId">ИД агента</param>
        /// <param name="model">параметры сотрудника</param>
        /// <returns>добавленную запись</returns>
        public IHttpActionResult PostToExistingAgent(int AgentPersonId, [FromBody]ModifyDictionaryAgentEmployee model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            model.Id = AgentPersonId;
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentEmployee, cxt, model));
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
                var cxt = DmsResolver.Current.Get<UserContext>().Get();
                var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
                tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentEmployee, cxt, model);
                return Get(model.Id);
            }

            /// <summary>
            /// Удаление сотрудника
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public IHttpActionResult Delete([FromUri] int id)
            {
                var cxt = DmsResolver.Current.Get<UserContext>().Get();
                var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

                tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentEmployee, cxt, id);
                FrontDictionaryAgentPerson tmp = new FrontDictionaryAgentPerson();
                tmp.Id = id;

                return new JsonResult(tmp, this);

            }
        }
}
