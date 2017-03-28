using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Аватарка пользователя
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserImageController : ApiController
    {

        /// <summary>
        /// Возвращает аватарку
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Image)]
        [ResponseType(typeof(FrontAgentEmployeeUser))]
        public async Task<IHttpActionResult> Get()
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItem = tmpService.GetDictionaryAgentUserPicture(context, context.CurrentAgentId);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Устанавливает новую аватарку
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Image)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyDictionaryAgentImage model)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Удаляет аватарку текущего пользователя
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Image)]
        public async Task<IHttpActionResult> Delete()
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                Action.Execute(context, EnumDictionaryActions.DeleteAgentImage, context.CurrentAgentId);
                var tmpItem = new FrontDeleteModel(context.CurrentAgentId);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

    }
}