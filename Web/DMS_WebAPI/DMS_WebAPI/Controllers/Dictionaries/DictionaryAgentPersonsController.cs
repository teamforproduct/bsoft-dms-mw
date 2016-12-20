using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.SystemCore;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "DictionaryAgentPersons")]
    public class DictionaryAgentPersonsController : ApiController
    {
       
        /// <summary>
        /// ПОлучить список физлиц
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns>коллекцию контрагентов</returns>
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentPerson filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentPersons(ctx, filter,paging);
            var res= new JsonResult(tmpDicts, this);
            res.Paging = paging;
            return res;
        }

       /// <summary>
       /// Получить физлицо по ИД
       /// </summary>
       /// <param name="id">ИД</param>
       /// <returns>запись справочника агентов-физлиц</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetAgentPerson(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// добавить физлицо
        /// </summary>
        /// <param name="model">параметры физлица</param>
        /// <returns>добавленную запись</returns>
        public IHttpActionResult Post([FromBody]AddAgentPerson model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentPerson, ctx, model));
        }


        /// <summary>
        /// добавить реквизиты физлица к существующему контрагенту
        /// </summary>
        /// <param name="AgentId">ИД агента</param>
        /// <param name="model">параметры физлица</param>
        /// <returns>добавленную запись</returns>
        //public IHttpActionResult PostToExistingAgent(int AgentId,[FromBody]ModifyDictionaryAgentPerson model)
        //{
        //    var ctx = DmsResolver.Current.Get<UserContexts>().Get();
        //    var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
        //    model.Id = AgentId;
        //    return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentPerson, ctx, model));
        //}
        /// <summary>
        /// изменить физлицо. контрагент меняется, если он является только физлицом
        /// </summary>
        /// <param name="id">ИД</param>
        /// <param name="model">параметры</param>
        /// <returns>возвращает измененную запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyAgentPerson model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentPerson, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// удалить физлицо, контрагент удаляется,  если он является только физлицом
        /// </summary>
        /// <param name="id">ИД</param>
        /// <returns>ИД удаленной записи</returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentPerson, ctx, id);
            
             
            FrontDictionaryAgentPerson tmp = new FrontDictionaryAgentPerson();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }

    }
}