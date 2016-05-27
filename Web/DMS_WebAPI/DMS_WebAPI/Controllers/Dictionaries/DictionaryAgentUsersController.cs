using BL.CrossCutting.DependencyInjection;
using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryAgentUsersController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "AgentUsers"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Компании"</param>
        /// <returns>FrontDictionaryAgentUsers</returns>
        // GET: api/DictionaryAgentUsers
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentUser filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentUsers(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "AgentUsers" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryAgentUsers</returns>
        // GET: api/DictionaryAgentUsers/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgentUser(cxt, id);
            return new JsonResult(tmpDict, this);
        }


        /// <summary>
        /// Добавление записи в словаре "Компании"
        /// </summary>
        /// <param name="model">ModifyDictionaryAgentUser</param>
        /// <returns>DictionaryAgentUsers</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryAgentUser model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentUser, cxt, model));
        }

        /// <summary>
        /// Изменение записи в словаре "Компании"
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryAgentUser</param>
        /// <returns>DictionaryAgentUsers</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgentUser model)
        {
            // Спецификация REST требует отдельного указания ID, несмотря на то, что параметр ID есть в ModifyDictionaryAgentUser

            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentUser, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи в словаре "Компании"
        /// </summary>
        /// <returns>DictionaryAgentUsers</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentUser, cxt, id);
            FrontDictionaryAgentUser tmp = new FrontDictionaryAgentUser();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}