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
    {/// <summary>
    /// Контрагент - сотрудник
    /// </summary>
    [Authorize]
    public class DictionaryAgentEmployeesController : ApiController
    {
        
            public IHttpActionResult Get([FromUri] FilterDictionaryAgentEmployee filter)
            {
                var cxt = DmsResolver.Current.Get<UserContext>().Get();
                var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
                var tmpDicts = tmpDictProc.GetDictionaryAgentEmployees(cxt, filter);
                return new JsonResult(tmpDicts, this);
            }


            public IHttpActionResult Get(int id)
            {
                var cxt = DmsResolver.Current.Get<UserContext>().Get();
                var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
                var tmpDict = tmpDictProc.GetDictionaryAgentEmployee(cxt, id);
                return new JsonResult(tmpDict, this);
            }

            public IHttpActionResult Post([FromBody]ModifyDictionaryAgentEmployee model)
            {
                var cxt = DmsResolver.Current.Get<UserContext>().Get();
                var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
                return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentEmployee, cxt, model));
            }

            public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgentEmployee model)
            {
                model.Id = id;
                var cxt = DmsResolver.Current.Get<UserContext>().Get();
                var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
                tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentEmployee, cxt, model);
                return Get(model.Id);
            }


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
