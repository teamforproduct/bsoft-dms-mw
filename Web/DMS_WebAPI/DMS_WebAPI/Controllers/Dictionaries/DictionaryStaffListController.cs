using BL.Logic.DictionaryCore.Interfaces;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Common;

namespace DMS_WebAPI.Controllers.D
{
    /// <summary>
    /// Действия связанные с пользовательской настройкой системы
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/DictionaryStaffList")]
    public class DictionaryStaffListController : ApiController
    {
        /// <summary>
        /// Список элементов меню, доступный пользователю
        /// </summary>
        /// <returns>Меню</returns>
        [Route("GetStaffList")]
        [HttpGet]
        public IHttpActionResult GetStaffList([FromUri] DictionaryBaseFilterParameters filter, [FromUri] StartWith startWith)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetStaffList(ctx, filter, startWith);
            return new JsonResult(tmpItems, this);
        }

    }

    
  
}
