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
        /// Возвращает список КЛИЕНТСКИХ переводов
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        public IHttpActionResult Get([FromUri] FilterAdminLanguageValue filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ILanguageService>();
            var tmpItems = tmpService.GetAdminLanguageValues(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает список ЗАВОДСКИХ переводов
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [HttpGet]
        [Route("GetDefaults")]
        public IHttpActionResult GetDefaults([FromUri] FilterAdminLanguageValue filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ILanguages>();
            var tmpItems = tmpService.GetLanguageValues(filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает список ЗАВОДСКИХ локалей
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [HttpGet]
        [Route("GetLanguages")]
        public IHttpActionResult GetLanguages([FromUri] FilterAdminLanguage filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ILanguages>();
            var tmpItems = tmpService.GetLanguages(filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает список локалей c меткой для пользователя
        /// </summary>
        /// <param name="agentId">Пользователь</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserLanguages")]
        public IHttpActionResult GetUserLanguages([FromUri] int agentId, [FromUri] FilterAdminLanguage filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ILanguageService>();
            var tmpItems = tmpService.GetAdminUserLanguages(ctx, agentId, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возврат к заводским переводам (Клиентские переводы остаются и имеют привилегии при переводе)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RefreshLanguageValues")]
        public IHttpActionResult RefreshLanguageValues()
        {
            //var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<ILanguages>();
            tmpService.RefreshLanguageValues();
            return new JsonResult("Done", this);
        }

       

        
    }
}