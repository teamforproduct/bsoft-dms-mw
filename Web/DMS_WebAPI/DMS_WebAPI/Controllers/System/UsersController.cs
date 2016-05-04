using BL.Logic.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Exception;
using BL.Logic.DictionaryCore.Interfaces;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/v2/Users")]
    public class UsersController : ApiController
    {
        /// <summary>
        /// Получение информации о пользователе
        /// </summary>
        /// <returns>список должностей</returns>
        [Route("UserInfo")]
        [HttpGet]
        public IHttpActionResult GetUserInfo()
        {
            var context = DmsResolver.Current.Get<UserContext>().Get();
            var dicProc = DmsResolver.Current.Get<IDictionaryService>();

            var agent = dicProc.GetDictionaryAgent(context, context.CurrentAgentId);

            return new JsonResult(agent, this);
        }

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
            var user_context = DmsResolver.Current.Get<UserContext>();
            var context = user_context.Get();
            var admProc = DmsResolver.Current.Get<IAdminService>();
            admProc.VerifyAccess(context, new VerifyAccess() { PositionsIdList = positionsIdList });
            user_context.SetUserPositions(context.CurrentEmployee.Token, positionsIdList);
            //context.CurrentPositionsIdList = positionsIdList;
            //cxt.CurrentPositions = new List<CurrentPosition>() { new CurrentPosition { CurrentPositionId = positionId } };
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Получение списка доступных серверов
        /// </summary>
        /// <returns>список серверов</returns>
        [Route("Servers")]
        [HttpGet]
        public IHttpActionResult GetServers()
        {
            var context = DmsResolver.Current.Get<UserContext>().Get();
            return new JsonResult(new Servers().GetServersByUser(context.CurrentEmployee.UserId), this);
        }

        /// <summary>
        /// Установить сервер для использования
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        [Route("Servers")]
        [HttpPost]
        public IHttpActionResult SetServers([FromBody]int serverId)
        {
            var mngContext = DmsResolver.Current.Get<UserContext>();

            var db = new Servers().GetServer(serverId);
            if (db == null)
            {
                throw new DatabaseIsNotFound();
            }

            mngContext.Set(db);


            return new JsonResult(null, this);
        }
    }
}
