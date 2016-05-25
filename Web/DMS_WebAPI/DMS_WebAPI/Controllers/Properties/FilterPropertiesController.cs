using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.SystemCore.Filters;
using BL.Logic.PropertyCore.Interfaces;

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            var tmpItems = tmpServ.GetFilterProperties(ctx, filter);
            return new JsonResult(tmpItems, this);
        }
    }
}