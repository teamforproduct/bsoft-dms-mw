using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.SystemCore.Filters;
using System.Web.Http.Description;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.FrontModel;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers
{
    /// <summary>
    /// Форматы значений для формул
    /// </summary>
    [Authorize]
    public class SystemSettingsController : ApiController
    {
        /// <summary>
        /// Список настроек
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        // GET: api/SystemFormats
        public IHttpActionResult Get([FromUri] FilterSystemSetting filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();
            var tmpItem = tmpService.GetSystemSettings(ctx, filter);
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Устанавливает значение настойки
        /// </summary>
        /// <param name="model">ModifyAdminPositionRole</param>
        /// <returns>FrontAdminPositionRole</returns>
        [ResponseType(typeof(FrontSystemSetting))]
        public IHttpActionResult Post([FromBody]ModifySystemSetting model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();
            var tmpItem = tmpService.ExecuteAction(EnumSystemActions.SetSetting, cxt, model);
            return Get(new FilterSystemSetting() { Key = (string)tmpItem });
        }

    }
}