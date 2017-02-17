using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// !!! Доступ не ограничен.
    /// Часто выбираемые элементы. 
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserFavoritesController : ApiController
    {

        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список часто выбираемых элементов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Favorites + "/Bulk")]
        [ResponseType(typeof(FrontUserFavorites))]
        public IHttpActionResult Get()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetUserFavourites(ctx);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }






        ///// <summary>
        ///// Возвращает отпечаток по Id
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route(Features.Favorites + "/{Id:int}")]
        //[ResponseType(typeof(InternalAgentFavourite))]
        //public IHttpActionResult Get(int Id)
        //{
        //    if (!stopWatch.IsRunning) stopWatch.Restart();
        //    var ctx = DmsResolver.Current.Get<UserContexts>().Get();
        //    var webService = DmsResolver.Current.Get<WebAPIService>();
        //    var tmpItem = webService.  fgGetUserFingerprint(Id);
        //    var res = new JsonResult(tmpItem, this);
        //    res.SpentTime = stopWatch;
        //    return res;
        //}


        /// <summary>
        /// Добавляет часто выбираемый элемент
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Favorites + "/Bulk")]
        public IHttpActionResult PostBulk([FromBody]FrontUserFavorites model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.SetUserFavoritesBulk(ctx, model);
            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет часто выбираемый элемент
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Favorites)]
        public IHttpActionResult Post([FromBody]AddAgentFavourite model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.SetUserFavorite(ctx, model);
            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Удаляет часто выбираемый элемент
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Favorites + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            webService.DeleteUserFingerprint(Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;

        }
    }
}