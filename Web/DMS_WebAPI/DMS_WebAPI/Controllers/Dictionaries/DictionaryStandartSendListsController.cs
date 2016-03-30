using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryStandartSendListsController : ApiController
    {
        // GET: api/DictionaryStandartSendLists
        public IHttpActionResult Get([FromUri] FilterDictionaryStandartSendList filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryStandartSendLists(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryStandartSendLists/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryStandartSendList(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}