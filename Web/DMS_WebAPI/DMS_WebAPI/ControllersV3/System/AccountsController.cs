using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using BL.Model.Users;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Типы контактов
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Accounts)]
    public class AccountsController : WebApiController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Create")]
        public async Task<IHttpActionResult> Create(Account model)
        {

            var tmpService = DmsResolver.Current.Get<WebAPIService>();
            await tmpService.RegisterUser(model);
            return new JsonResult(null, this);
        }

    }
}