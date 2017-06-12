using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Часто выбираемые элементы. 
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserFavoritesController : WebApiController
    {

        /// <summary>
        /// Возвращает список часто выбираемых элементов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Favorites + "/Bulk")]
        [ResponseType(typeof(FrontUserFavorites))]
        public async Task<IHttpActionResult> Get()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetUserFavourites(context);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        ///// <summary>
        ///// Возвращает отпечаток по Id
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route(Features.Favorites + "/{Id:int}")]
        //[ResponseType(typeof(InternalAgentFavourite))]
        //public async Task<IHttpActionResult> Get(int Id)
        //{
        //    var ctx = DmsResolver.Current.Get<UserContexts>().Get();
        //    var webService = DmsResolver.Current.Get<WebAPIService>();
        //    var tmpItem = webService.  fgGetUserFingerprint(Id);
        //    var res = new JsonResult(tmpItem, this);
        //    return res;
        //}


        /// <summary>
        /// Добавляет часто выбираемый элемент
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Favorites + "/Bulk")]
        public async Task<IHttpActionResult> PostBulk([FromBody]FrontUserFavorites model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                tmpService.SetUserFavoritesBulk(context, model);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// Добавляет часто выбираемый элемент
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Favorites)]
        public async Task<IHttpActionResult> Post([FromBody]AddAgentFavourite model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                tmpService.SetUserFavorite(context, model);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// Удаляет часто выбираемый элемент
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Favorites + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                //tmpService.DeleteUserFav(Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }
    }
}