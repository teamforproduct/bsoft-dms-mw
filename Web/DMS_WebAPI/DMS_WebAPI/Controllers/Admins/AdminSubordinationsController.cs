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
using System.Diagnostics;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.SystemCore.Filters;
using BL.Model.Constants;
using BL.CrossCutting.Interfaces;
using BL.Model.Tree;

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Описывает возможности должностей выполнять рассылку документов на другие должности
    /// Рассылка бывает двух типов: для исполнения и для сведения
    /// Этот функционал по умолчанию выключен для нового клиента, рассылка разрешена на все должности. 
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "AdminSubordinations")]
    public class AdminSubordinationsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список должностей на которых разрешена рассылка документов
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [ResponseType(typeof(List<FrontAdminSubordination>))]
        public IHttpActionResult Get([FromUri] FilterAdminSubordination filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAdminSubordinations(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// GetAdminSubordinations by ID 
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>FrontAdminSubordination</returns>
        [ResponseType(typeof(FrontAdminSubordination))]
        public IHttpActionResult Get(int id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetAdminSubordinations(ctx, new FilterAdminSubordination() { IDs = new List<int> { id } });
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает список должностей с пычками для управления рассылкой для сведения и для исполнения
        /// </summary>
        /// <param name="positionId"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSubordinationsDIP")]
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult GetSubordinationsDIP([FromUri] int positionId, [FromUri] FilterAdminSubordinationTree filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetSubordinationsDIP(ctx, positionId, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Разрешает должности выполнять рассылку на другую должность с учетом типа рассылки
        /// </summary>
        /// <param name="model">ModifyAdminSubordination</param>
        /// <returns>FrontAdminSubordination</returns>
        public IHttpActionResult Post([FromBody] ModifyAdminSubordination model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetSubordination, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Копирует настроки рассылки от Source к Target должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DuplicateSubordinations")]
        public IHttpActionResult DuplicateSubordinations([FromBody] CopyAdminSettingsByPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.DuplicateSubordinations, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
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
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetSubordinationByDepartment, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
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
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetSubordinationByCompany, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Управление рассылкой для должности в масштабах компании
        /// Разрешает выполнять рассылку для сведения или исполнения для всех сотрудников компании
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetDefault")]
        public IHttpActionResult SetDefault([FromBody] ModifyAdminDefaultByPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetDefaultSubordination, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Управление рассылкой для должности в масштабах компании
        /// Разрешает выполнять рассылку для сведения или исполнения для всех сотрудников компании
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetAll")]
        public IHttpActionResult SetAll([FromBody] ModifyAdminSubordinations model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetAllSubordination, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// Если флаг TRUE, то при создании новой должности устанавливается рассылку на все должности для исполнения, 
        /// в противном случае (FALSE) рассылка устанавливается в дефолт
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("IsSendAllForExecution")] //IsSetDefaultsForExecution
        public IHttpActionResult IsSendAllForExecution()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetSubordinationsSendAllForExecution(cxt);//, new FilterSystemSetting() { Key = SettingConstants.SUBORDINATIONS_SEND_ALL_FOR_EXECUTION });
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// Если флаг TRUE, то при создании новой должности устанавливается рассылку на все должности для сведения, 
        /// в противном случае (FALSE) рассылка устанавливается в дефолт
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("IsSendAllForInforming")]
        public IHttpActionResult IsSendAllForInforming()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetSubordinationsSendAllForInforming(cxt);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}