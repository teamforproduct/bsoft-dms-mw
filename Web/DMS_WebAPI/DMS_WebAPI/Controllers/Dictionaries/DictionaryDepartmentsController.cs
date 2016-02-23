using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryDepartmentsController : ApiController
    {
        // GET: api/DictionaryDepartments
        public IHttpActionResult Get([FromUri] FilterDictionaryDepartment filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryDepartments(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryDepartments/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryDepartment(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}