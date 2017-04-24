using BL.CrossCutting.DependencyInjection;
using BL.Model.AdminCore.Clients;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
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
        /// Создает заявку на добавление нового клиента
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult AddClientRequest([FromBody]AddClientSaaS model)
        {
            var tmpService = DmsResolver.Current.Get<WebAPIService>();
            tmpService.AddClientSaaSRequest(model);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Создает нового клиента
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create/ByHash")]
        public IHttpActionResult AddClient([FromBody]AddClientFromHash model)
        {
            var tmpService = DmsResolver.Current.Get<WebAPIService>();
            tmpService.AddClientByEmail(model);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Создает нового клиента
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create/BySMS")]
        public IHttpActionResult AddClient([FromBody]AddClientFromSMS model)
        {
            var tmpService = DmsResolver.Current.Get<WebAPIService>();
            tmpService.AddClientBySMS(model);
            return new JsonResult(null, this);
        }


        /// <summary>
        /// Удаляет данные клиента
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            var tmpService = DmsResolver.Current.Get<WebAPIService>();
            tmpService.DeleteClient(Id);
            return new JsonResult(null, this);
        }

    }
}