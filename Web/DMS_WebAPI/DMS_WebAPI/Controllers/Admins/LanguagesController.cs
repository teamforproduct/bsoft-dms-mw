using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;

namespace DMS_WebAPI.Controllers.Admins
{
    [Authorize]
    [RoutePrefix("api/v2/Languages")]
    public class LanguagesController : ApiController
    {
        /// <summary>
        /// Возвращает список переводов
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        public IHttpActionResult Get([FromUri] FilterAdminLanguageValue filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<ILanguageService>();
            var tmpItems = tmpService.GetAdminLanguageValues(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        [Route("RefreshLanguageValues")]
        public IHttpActionResult RefreshLanguageValues()
        {
            //var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<Languages>();
            tmpService.RefreshLanguageValues();
            return new JsonResult("Done", this);
        }

       

        
    }
}