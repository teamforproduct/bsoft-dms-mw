using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Tree;
using System.Web.Http.Description;
using System.Collections.Generic;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "DictionaryRegistrationJournals")]
    public class DictionaryRegistrationJournalsController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Журналы регистрации"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Журналы регистрации"</param>
        /// <returns>FrontDictionaryRegistrationJournals</returns>
        // GET: api/DictionaryRegistrationJournals
        public IHttpActionResult Get([FromUri] FilterDictionaryRegistrationJournal filter)
        {
            return new JsonResult(null, this);
        }

       
    }
}