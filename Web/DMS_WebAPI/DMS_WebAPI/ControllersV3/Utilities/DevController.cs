using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemCore.Interfaces;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

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
        public async Task<IHttpActionResult> Get()
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
        public async Task<IHttpActionResult> GetVersion()
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
        public async Task<IHttpActionResult> GetRequest()
        {
            return new JsonResult(HttpContext.Current.Request, this);
        }

        /// <summary>
        /// Обновляет настройку доступов
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RefreshPermissions")]
        public async Task<IHttpActionResult> RefreshPermissions()
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ISystemService>();
                tmpService.RefreshModuleFeature(context);

                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RefreshSystemActions")]
        public async Task<IHttpActionResult> RefreshSystemActions()
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ISystemService>();
                tmpService.RefreshSystemActions(context);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RefreshSystemObjects")]
        public async Task<IHttpActionResult> RefreshSystemObjects()
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ISystemService>();
                tmpService.RefreshSystemObjects(context);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращако количество открытых сессий
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SessionsCount")]
        public async Task<IHttpActionResult> GetUserContextsCount()
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
        public async Task<IHttpActionResult> Test()
        {
            //TODO REMOVE
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var ftService = DmsResolver.Current.Get<IFullTextSearchService>();
                //ftService.FullTextIndexPrepareNew(context, EnumObjects.Documents, true, true, 0, 1000);
                return new JsonResult(null, this);
            });
        }

    }
}