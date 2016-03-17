using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    [Authorize]
    public class DictionaryAgentPersonsController : ApiController
    {
       
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentPerson filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentPersons(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

       
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgentPerson(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        public IHttpActionResult Post([FromBody]ModifyDictionaryAgentPerson model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentPerson, cxt, model));
        }

        public IHttpActionResult PostToExistingAgent(int AgentId,[FromBody]ModifyDictionaryAgentPerson model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            model.Id = AgentId;
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentPerson, cxt, model));
        }

        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgentPerson model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentPerson, cxt, model);
            return Get(model.Id);
        }

      
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentPerson, cxt, id);
            FrontDictionaryAgentPerson tmp = new FrontDictionaryAgentPerson();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }

    }
}