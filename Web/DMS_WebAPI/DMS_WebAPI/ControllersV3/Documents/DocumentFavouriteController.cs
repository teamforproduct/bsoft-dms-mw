using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Threading.Tasks;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentFavouriteController : ApiController
    {
        /// <summary>
        /// Добавляет в Избранное
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Favourite + "/Add")]
        [HttpPut]
        public async Task<IHttpActionResult> AddFavourite(ChangeFavourites model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                Action.Execute(context, EnumDocumentActions.AddFavourite, model, model.CurrentPositionId);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// Удаляет из Избранного
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Favourite + "/Delete")]
        [HttpPut]
        public async Task<IHttpActionResult> DeleteFavourite(ChangeFavourites model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                Action.Execute(context, EnumDocumentActions.DeleteFavourite, model, model.CurrentPositionId);
                var res = new JsonResult(null, this);
                return res;
            });
        }
    }
}
