using BL.CrossCutting.DependencyInjection;
using BL.Model.AdminCore.Clients;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Threading.Tasks;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.Clients
{
    /// <summary>
    /// Клиенты
    /// </summary>
    //![Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Clients)]
    public class ClientsInfoController : WebApiController
    {

        /// <summary>
        /// Создает нового клиента
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddClientSaaS model)
        {
            var tmpService = DmsResolver.Current.Get<WebAPIService>();
            tmpService.AddClientSaaS(model);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Удаляет банк
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            var tmpService = DmsResolver.Current.Get<WebAPIService>();
            tmpService.DeleteClient(Id);
            return new JsonResult(null, this);
        }

    }
}