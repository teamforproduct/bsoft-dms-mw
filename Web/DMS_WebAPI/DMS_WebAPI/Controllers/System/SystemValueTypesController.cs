using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.SystemCore.Filters;

namespace DMS_WebAPI.Controllers
{
    /// <summary>
    /// Типы значений
    /// </summary>
    [Authorize]
    public class SystemValueTypesController : ApiController
    {
        /// <summary>
        /// Список типов значений
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        // GET: api/SystemFormats
        public IHttpActionResult Get([FromUri] FilterSystemValueType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ISystemService>();
            var tmpDicts = tmpSysProc.GetSystemValueTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

    }
}