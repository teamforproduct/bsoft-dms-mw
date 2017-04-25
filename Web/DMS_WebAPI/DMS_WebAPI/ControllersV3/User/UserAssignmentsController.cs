using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Назначения
    /// Пользователь-сотрудник может работать в системе если назначен хотя бы на одну должность в текущем интервале времени
    /// Пользователь-сотрудник может одновременно занимать должность и исполнять обязанноти другой должности или реферировать
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserAssignmentsController : WebApiController
    {
        /// <summary>
        /// Возвращает список назначений для текущего пользователя (должность - интервал назначения, количество новых событий)
        /// Пользоателю может быть назначено исполнение обязанностей другой должности
        /// </summary>
        /// <returns>список должностей</returns>
        [HttpGet]
        [Route(Features.Assignments + "/Current")]
        [ResponseType(typeof(List<FrontUserAssignments>))]
        public async Task<IHttpActionResult> Assignments()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IAdminService>();
                var tmpItems = tmpService.GetAvailablePositions(context);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает список назначений для текущего пользователя (должность - интервал назначения, количество новых событий)
        /// Пользоателю может быть назначено исполнение обязанностей другой должности
        /// </summary>
        /// <returns>список должностей</returns>
        [HttpGet]
        [Route(Features.Assignments + "/Available")]
        [ResponseType(typeof(List<FrontUserAssignmentsAvailableGroup>))]
        public async Task<IHttpActionResult> GetAvailableShort()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IAdminService>();
                var tmpItems = tmpService.GetAvailablePositionsDialog(context);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }



        /// <summary>
        /// Возвращает список назначений, от которых пользователь сейчас работатет.
        /// </summary>
        /// <returns>массива ИД должностей</returns>
        [HttpGet]
        [Route(Features.Assignments)]
        [ResponseType(typeof(List<FrontUserAssignments>))]
        public async Task<IHttpActionResult> GetAssignments()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IAdminService>();
                var tmpItems = tmpService.GetAvailablePositions(context, context.CurrentPositionsIdList);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Устанавливает назначения из списка доступных.
        /// Пользователь может переключаться между доступными назначениями для разделения информационного потока или выполнять обязанности от всех назначений. 
        /// </summary>
        /// <param name="positionsIdList">ИД должности</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Assignments)]
        public async Task<IHttpActionResult> Assignments([FromBody]List<int> positionsIdList)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var userContexts = (UserContexts)param;
                var tmpService = DmsResolver.Current.Get<IAdminService>();
                tmpService.VerifyAccess(context, new VerifyAccess() { PositionsIdList = positionsIdList });

                //TODO Здесь необходима проверка на то, что список должностей из доступных
                userContexts.SetUserPositions(context.Employee.Token, positionsIdList);
                //context.CurrentPositionsIdList = positionsIdList;
                //ctx.CurrentPositions = new List<CurrentPosition>() { new CurrentPosition { CurrentPositionId = positionId } };

                return new JsonResult(null, this);
            }, DmsResolver.Current.Get<UserContexts>());
        }


    }
}