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
    /// Объекты системы
    /// </summary>
    [Authorize]
    public class SystemObjectsController : ApiController
    {
        /// <summary>
        /// Список объектов системы
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        // GET: api/SystemObjects
        public IHttpActionResult Get([FromUri] FilterSystemObject filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ISystemService>();
            var tmpDicts = tmpSysProc.GetSystemObjects(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

    }
}