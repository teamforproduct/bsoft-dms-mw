using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;
using BL.Model.DictionaryCore.FrontModel;

namespace DMS_WebAPI.Controllers.Dictionaries
{

    /// <summary>
    /// Системный справочник, описывает типы документов по направлению:
    /// 1 - Входящие;
    /// 2 -	Исходящие;
    /// 3 -	Внутренние;
    /// </summary>
    [Authorize]
    public class DictionaryDocumentDirectionsController : ApiController
    {
        /// <summary>
        /// Направления документов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [ResponseType(typeof(List<FrontDictionaryDocumentDirection>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryDocumentDirection filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryDocumentDirections(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Направление документа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(FrontDictionaryDocumentDirection))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryDocumentDirection(ctx, id);
            return new JsonResult(tmpDict, this);
        }
    }
}