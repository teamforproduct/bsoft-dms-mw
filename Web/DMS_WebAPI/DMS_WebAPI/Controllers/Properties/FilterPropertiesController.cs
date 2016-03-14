using BL.Logic.DependencyInjection;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using BL.Logic.PropertyCore.Interfaces;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.FrontModel;

namespace DMS_WebAPI.Controllers.Properties
{
    [Authorize]
    public class FilterPropertiesController : ApiController
    {
        /// <summary>
        /// Получить список фильтров для дополнительных свойств
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Список фильтров</returns>
        public IHttpActionResult Get([FromUri] FilterProperties filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            var tmpItems = tmpServ.GetFilterProperties(cxt, filter);
            return new JsonResult(tmpItems, this);
        }
    }
}