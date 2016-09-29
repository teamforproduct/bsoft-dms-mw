using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Описывает возможности должностей выполнять рассылку документов на другие должности
    /// Рассылка бывает двух типов: для исполнения и для сведения
    /// Этот функционал по умолчанию выключен для нового клиента, рассылка разрешена на все должности. 
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/AdminSubordinations")]
    public class AdminSubordinationsController : ApiController
    {
        /// <summary>
        /// Возвращает список должностей на которых разрешена рассылка документов
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [ResponseType(typeof(List<FrontAdminSubordination>))]
        public IHttpActionResult Get([FromUri] FilterAdminSubordination filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAdminSubordinations(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// GetAdminSubordinations by ID 
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>FrontAdminSubordination</returns>
        [ResponseType(typeof(FrontAdminSubordination))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetAdminSubordinations(ctx, new FilterAdminSubordination() { IDs = new List<int> { id } });
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Возвращает список должностей с пычками для управления рассылкой для сведения и для исполнения
        /// </summary>
        /// <param name="positionId"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSubordinationsDIP")]
        public IHttpActionResult GetSubordinationsDIP([FromUri] int positionId, [FromUri] FilterAdminSubordinationTree filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetSubordinationsDIP(ctx, positionId, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Копирует настроки рассылки от Source к Target должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DuplicateSubordinations")]
        public IHttpActionResult DuplicateSubordinations([FromBody] CopyAdminSubordinations model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.DuplicateSubordinations, cxt, model);
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Управление рассылкой для должности в масштабах отдела и дочерних отделов
        /// Разрешает выполнять рассылку для сведения или исполнения для всех сотрудников отдела и дочерних отделов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetByDepartment")]
        public IHttpActionResult SetByDepartment([FromBody] ModifyAdminSubordinationByDepartment model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetSubordinationByDepartment, cxt, model);
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Управление рассылкой для должности в масштабах компании
        /// Разрешает выполнять рассылку для сведения или исполнения для всех сотрудников компании
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetByCompany")]
        public IHttpActionResult SetByCompany([FromBody] ModifyAdminSubordinationByCompany model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetSubordinationByCompany, cxt, model);
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Разрешает должности выполнять рассылку на другую должность с учетом типа расслки
        /// </summary>
        /// <param name="model">ModifyAdminSubordination</param>
        /// <returns>FrontAdminSubordination</returns>
        public IHttpActionResult Post([FromBody] ModifyAdminSubordination model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetSubordination, cxt, model);
            return new JsonResult(tmpItem, this);
        }

    }
}