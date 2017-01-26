using BL.Model.AdminCore;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.Controllers.Admins
{
    [Authorize]
    public class AdminAccessLevelsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Получение словаря уровней доступа
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Список уровней доступа</returns>
        [ResponseType(typeof(List<FrontAdminAccessLevel>))]
        public IHttpActionResult Get([FromUri] FilterAdminAccessLevel filter)
        {
            var res = new JsonResult(null, this);
            return res;
        }

    }
}