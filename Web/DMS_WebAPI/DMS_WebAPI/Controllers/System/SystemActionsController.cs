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
    /// Действия системы
    /// </summary>
    [Authorize]
    public class SystemActionsController : ApiController
    {
        /// <summary>
        /// Список действий системы
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        // GET: api/SystemActions
        public IHttpActionResult Get([FromUri] FilterSystemAction filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ISystemService>();
            var tmpDicts = tmpSysProc.GetSystemActions(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

    }
}