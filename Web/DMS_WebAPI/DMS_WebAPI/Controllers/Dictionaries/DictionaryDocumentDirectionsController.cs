using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{

    /// <summary>
    /// Yfghfdktybz ljrevtynjd
    /// </summary>
    [Authorize]
    public class DictionaryDocumentDirectionsController : ApiController
    {
        /// <summary>
        /// Направления документов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        // GET: api/DictionaryDocumentDirections
        public IHttpActionResult Get([FromUri] FilterDictionaryDocumentDirection filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryDocumentDirections(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Направление документа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/DictionaryDocumentDirections/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryDocumentDirection(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}