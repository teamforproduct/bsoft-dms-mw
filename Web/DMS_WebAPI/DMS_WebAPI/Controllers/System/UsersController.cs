using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore;
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
        /// <returns>список должностей</returns>
        [Route("AvailablePositions")]
        [HttpGet]
        public IHttpActionResult AvailablePositions()
        {
            var context = DmsResolver.Current.Get<UserContext>().Get();
            var admProc = DmsResolver.Current.Get<IAdminService>();
            return new JsonResult(admProc.GetPositionsByCurrentUser(context), this);
        }

        /// <summary>
        /// Получение массива ИД должностей, выбранных текущим пользователем
        /// </summary>
        /// <returns>массива ИД должностей</returns>
        [Route("ChoosenPositions")]
        [HttpGet]
        public IHttpActionResult ChoosenPositions()
        {
            var context = DmsResolver.Current.Get<UserContext>().Get();
            return new JsonResult(context.CurrentPositionsIdList, this);
        }

        /// <summary>
        /// Установка должностей, от которых будет работать текущий пользователь
        /// </summary>
        /// <param name="positionsIdList">ИД должности</param>
        /// <returns></returns>
        [Route("ChoosenPositions")]
        public IHttpActionResult Post([FromBody]List<int> positionsIdList)
        {
            var context = DmsResolver.Current.Get<UserContext>().Get();
            var admProc = DmsResolver.Current.Get<IAdminService>();
            admProc.VerifyAccess(context, new VerifyAccess() { PositionsIdList = positionsIdList });
            context.CurrentPositionsIdList = positionsIdList;
            //cxt.CurrentPositions = new List<CurrentPosition>() { new CurrentPosition { CurrentPositionId = positionId } };
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
