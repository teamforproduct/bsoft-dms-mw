using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Users;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/v2/Users")]
    public class UsersController : ApiController
    {
        /// <summary>
        /// Получение списка должностей, доступных текущего для пользователя
        /// </summary>
        /// <returns></returns>
        [Route("Position")]
        public IHttpActionResult Get()
        {
            var context = DmsResolver.Current.Get<UserContext>().Get();
            var admProc = DmsResolver.Current.Get<IAdminService>();
            return new JsonResult(admProc.GetPositionsByCurrentUser(context), this);
        }

        /// <summary>
        /// Установка должности, от которой будет работать текущий пользователь
        /// </summary>
        /// <param name="positionId">ИД должности</param>
        /// <returns></returns>
        [Route("Position")]
        public IHttpActionResult Post([FromBody]int positionId)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            cxt.CurrentPosition = new List<Position>() { new Position { PositionId = positionId } };
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Получение списка доступных серверов
        /// </summary>
        /// <returns>список серверов</returns>
        [Route("Servers")]
        [AllowAnonymous]
        public IHttpActionResult GetServers()
        {
            var readXml = new ReadXml("/servers.xml");
            return new JsonResult(readXml.ReadDBsByUI(), this);
        }
    }
}
