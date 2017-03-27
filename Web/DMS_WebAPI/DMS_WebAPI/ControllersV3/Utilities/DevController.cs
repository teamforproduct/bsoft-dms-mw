using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BL.Logic.SystemServices.FullTextSearch;

namespace DMS_WebAPI.ControllersV3.Utilities
{
    /// <summary>
    /// Для разработки
    /// </summary>
    //![Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Dev)]
    public class DevController : ApiController
    {
        /// <summary>
        /// Возвращает Hello, world!
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Test")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get()
        {
            var res = new JsonResult("Hello, world!", this);
            return res;
        }

        /// <summary>
        /// Возвращает версию сборки
        /// </summary>
        /// <returns>FrontAdminPositions</returns>
        [HttpGet]
        [Route("Version")]
        public IHttpActionResult GetVersion()
        {
            var tmpItems = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает объект Request. Для определения параметров, которые браузер отправляет в запрос
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Request")]
        public IHttpActionResult GetRequest()
        {
            return new JsonResult(HttpContext.Current.Request, this);
        }

        /// <summary>
        /// Обновляет настройку доступов
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RefreshPermissions")]
        public IHttpActionResult RefreshPermissions()
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();

            tmpService.RefreshModuleFeature(ctx);

            var res = new JsonResult(null, this);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RefreshSystemActions")]
        public IHttpActionResult RefreshSystemActions()
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();
            tmpService.RefreshSystemActions(cxt);
            var res = new JsonResult(null, this);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RefreshSystemObjects")]
        public IHttpActionResult RefreshSystemObjects()
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();
            tmpService.RefreshSystemObjects(cxt);
            var res = new JsonResult(null, this);
            return res;
        }

        /// <summary>
        /// Возвращако количество открытых сессий
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SessionsCount")]
        public IHttpActionResult GetUserContextsCount()
        {
            var count = DmsResolver.Current.Get<UserContexts>().Count;
            return new JsonResult(count, this);
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("FullTextIndexPrepareNew")]
        public IHttpActionResult Test()
        {
            //TODO REMOVE
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var ftService = DmsResolver.Current.Get<IFullTextSearchService>();
            //ftService.FullTextIndexPrepareNew(cxt, EnumObjects.Documents, true, true, 0, 1000);
            return new JsonResult(null, this);
        }

    }
}