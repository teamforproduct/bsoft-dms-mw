using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore;

namespace DMS_WebAPI.Controllers
{
    /// <summary>
    /// Форматы значений для формул
    /// </summary>
    [Authorize]
    public class SystemLogsController : ApiController
    {
        /// <summary>
        /// Список логов
        /// </summary>
        /// <param name="filter">модель фильтров лога</param>
        /// <param name="paging">параметры пейджинга</param>
        /// <returns></returns>
        // GET: api/SystemFormats
        public IHttpActionResult Get([FromUri] FilterSystemLog filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ILogger>();
            var tmpDicts = tmpSysProc.GetSystemLogs(ctx, filter, paging);
            return new JsonResult(tmpDicts, this);
        }

    }
}