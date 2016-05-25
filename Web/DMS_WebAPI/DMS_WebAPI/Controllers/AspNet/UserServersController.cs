using DMS_WebAPI.Results;
using System.Web.Http;
using BL.Model.AspNet.IncomingModel;
using BL.Model.AspNet.FrontModel;
using DMS_WebAPI.Utilities.AspNet;
using BL.Model.AspNet.Filters;

namespace DMS_WebAPI.Controllers.AspNet
{
    [Authorize]
    public class UserServersController : ApiController
    {
        /// <summary>
        /// Получение списка пользователей на серверах
        /// </summary>
        /// <returns>список клиентов</returns>
        public IHttpActionResult Get([FromUri]FilterAspNetUserServers filter)
        {
            return new JsonResult(new AspNetUserServers().GetUserServers(filter), this);
        }

        /// <summary>
        /// Получение пользователя на сервере
        /// </summary>
        /// <returns>список клиентов</returns>
        private IHttpActionResult Get(int id)
        {
            return new JsonResult(new AspNetUserServers().GetUserServer(id), this);
        }
        /// <summary>
        /// Добавит пользователя на сервер
        /// </summary>
        /// <returns>клиент</returns>
        public IHttpActionResult Post(ModifyAspNetUserServers model)
        {
            return Get(new AspNetUserServers().AddUserServer(model));
        }

        /// <summary>
        /// Удаление пользователя на сервере
        /// </summary>
        /// <param name="id">ID записи полученой из общего списка</param>
        /// <returns>клиент</returns>
        public IHttpActionResult Delete(int id)
        {
            new AspNetUserServers().DeleteUserServer(id);
            var item = new FrontAspNetUserServers { Id = id };
            return new JsonResult(item, this);
        }
    }
}
