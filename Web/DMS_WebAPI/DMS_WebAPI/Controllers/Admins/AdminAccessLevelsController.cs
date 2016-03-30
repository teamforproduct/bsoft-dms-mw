using BL.Model.AdminCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;

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
            var dictSrv = DmsResolver.Current.Get<IDictionaryService>();
            var accLevels = dictSrv.GetAdminAccessLevels(cxt, filter);
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
            var dictSrv = DmsResolver.Current.Get<IDictionaryService>();
            var accLevel = dictSrv.GetAdminAccessLevel(cxt, id);
            return new JsonResult(accLevel, this);
        }
    }
}