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


        [HttpGet]
        [Route("GetSubordinationsTree")]
        public IHttpActionResult GetSubordinationsTree([FromUri] int positionId, [FromUri] FilterAdminSubordinationTree filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetSubordinationsTree(ctx, positionId, filter);
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
        /// Разрешает должности выполнять рассылку на другую должность с учетом типа расслки
        /// </summary>
        /// <param name="model">ModifyAdminSubordination</param>
        /// <returns>FrontAdminSubordination</returns>
        //public IHttpActionResult Post([FromBody] ModifyAdminSubordination model)
        //{
        //    var cxt = DmsResolver.Current.Get<UserContext>().Get();
        //    var tmpService = DmsResolver.Current.Get<IAdminService>();
        //    var tmpItem = tmpService.ExecuteAction(EnumAdminActions.AddSubordination, cxt, model);
        //    return Get((int)tmpItem);
        //}

        /// <summary>
        /// Разрешает должности выполнять рассылку на другую должность с учетом типа расслки
        /// </summary>
        /// <param name="model">ModifyAdminSubordination</param>
        /// <returns>FrontAdminSubordination</returns>
        //[HttpPost]
        //[Route("AddMany")]
        public IHttpActionResult Post([FromBody] ModifyAdminSubordinations model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetSubordination, cxt, model);
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Изменяет возможность выполнения рассылки
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <param name="model">ModifyAdminSubordination</param>
        /// <returns>FrontAdminSubordination</returns>
        //public IHttpActionResult Put(int id, [FromBody]ModifyAdminSubordination model)
        //{
        //    model.Id = id;
        //    var cxt = DmsResolver.Current.Get<UserContext>().Get();
        //    var tmpService = DmsResolver.Current.Get<IAdminService>();
        //    tmpService.ExecuteAction(EnumAdminActions.ModifySubordination, cxt, model);
        //    return Get(model.Id);
        //}

        /// <summary>
        /// Запрещает должности выполнять рассылку документов на другую должность
        /// </summary>
        /// <returns>FrontAdminSubordination</returns> 
        //public IHttpActionResult Delete([FromUri] int id)
        //{
        //    var cxt = DmsResolver.Current.Get<UserContext>().Get();
        //    var tmpService = DmsResolver.Current.Get<IAdminService>();

        //    tmpService.ExecuteAction(EnumAdminActions.DeleteSubordination, cxt, id);
        //    FrontAdminSubordination tmpItem = new FrontAdminSubordination() { Id = id };
        //    return new JsonResult(tmpItem, this);
        //}
    }
}