using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryStandartSendListContentsController : ApiController
    {
        // GET: api/DictionaryStandartSendListContents
        public IHttpActionResult Get([FromUri] FilterDictionaryStandartSendListContent filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryStandartSendListContents(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryStandartSendListContents/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryStandartSendListContent(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}