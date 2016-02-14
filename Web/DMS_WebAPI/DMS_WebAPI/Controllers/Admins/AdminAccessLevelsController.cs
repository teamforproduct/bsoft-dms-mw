using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Admins
{
    [Authorize]
    public class AdminAccessLevelsController : ApiController
    {
        /// <summary>
        /// Получение словаря уровней доступа
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Список уровней доступа</returns>
        public IHttpActionResult Get([FromUri] FilterAdminAccessLevel filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var admProc = DmsResolver.Current.Get<IAdminService>();
            var accLevels = admProc.GetAdminAccessLevels(cxt, filter);
            return new JsonResult(accLevels, this);
        }

        /// <summary>
        /// Получение словаря уровней доступа по ИД
        /// </summary>
        /// <param name="id">ИД уровня доступа</param>
        /// <returns>Уровень доуступа</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var admProc = DmsResolver.Current.Get<IAdminService>();
            var accLevel = admProc.GetAdminAccessLevel(cxt, id);
            return new JsonResult(accLevel, this);
        }
    }
}