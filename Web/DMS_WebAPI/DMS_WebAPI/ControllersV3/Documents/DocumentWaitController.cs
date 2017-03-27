﻿using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. Контроль.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentWaitController : ApiController
    {
        /// <summary>
        /// Возвращает список контролей
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Waits + "/Main")]
        [ResponseType(typeof(List<FrontDocumentWait>))]
        public IHttpActionResult PostGetList([FromBody]IncomingBase model)
        {
            if (model == null) model = new IncomingBase();
            if (model.Filter == null) model.Filter = new FilterBase();
            if (model.Paging == null) model.Paging = new UIPaging();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetDocumentWaits(ctx, model.Filter, model.Paging);
            var res = new JsonResult(items, this);
            res.Paging = model.Paging;
            return res;
        }        

        /// <summary>
        /// Возвращает контроль по ИД
        /// </summary>
        /// <param name="Id">ИД контроля</param>
        /// <returns>Событие</returns>
        [HttpGet]
        [Route(Features.Waits + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentWait))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var item = docProc.GetDocumentEvent(ctx, Id);
            var res = new JsonResult(item, this);
            return res;
        }   

        /// <summary>
        /// Добавляет самокотроль для документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Waits + "/ControlOn")]
        public IHttpActionResult ControlOn([FromBody]ControlOn model)
        {
            Action.Execute(EnumDocumentActions.ControlOn, model, model.CurrentPositionId);
            var res = new JsonResult(true, this);
            return res;
        }

        /// <summary>
        /// Добавляет прошение о переносе сроков исполнения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/AskPostponeDueDate")]
        public IHttpActionResult AskPostponeDueDate([FromBody]AskPostponeDueDate model)
        {
            Action.Execute(EnumDocumentActions.AskPostponeDueDate, model);
            var res = new JsonResult(true, this);
            return res;
        }

        /// <summary>
        /// Добавляет контроль о выполнении поручения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/MarkExecution")]
        public IHttpActionResult MarkExecution([FromBody]SendEventMessage model)
        {
            Action.Execute(EnumDocumentActions.MarkExecution, model);
            var res = new JsonResult(true, this);
            return res;
        }



        /// <summary>
        /// Регистрирует отказ по прошению о переносе сроков исполнения 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/CancelPostponeDueDate")]
        public IHttpActionResult CancelPostponeDueDate([FromBody]SendEventMessage model)
        {
            Action.Execute(EnumDocumentActions.CancelPostponeDueDate, model);
            var res = new JsonResult(true, this);
            return res;
        }

        /// <summary>
        /// Регистрирует отказ в приеме результата
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/RejectResult")]
        public IHttpActionResult RejectResult([FromBody]SendEventMessage model)
        {
            Action.Execute(EnumDocumentActions.RejectResult, model);
            var res = new JsonResult(true, this);
            return res;
        }

        /// <summary>
        /// Регистрирует прием результата
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/AcceptResult")]
        public IHttpActionResult AcceptResult([FromBody]ControlOff model)
        {
            Action.Execute(EnumDocumentActions.AcceptResult, model);
            var res = new JsonResult(true, this);
            return res;
        }

        /// <summary>
        /// Регистрирует отмену поручения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/CancelExecution")]
        public IHttpActionResult CancelExecution([FromBody]ControlOff model)
        {
            Action.Execute(EnumDocumentActions.CancelExecution, model);
            var res = new JsonResult(true, this);
            return res;
        }



        /// <summary>
        /// Снимает самоконтроль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/ControlOff")]
        public IHttpActionResult ControlOff([FromBody]ControlOff model)
        {
            Action.Execute(EnumDocumentActions.ControlOff, model);
            var res = new JsonResult(true, this);
            return res;
        }


        /// <summary>
        /// Изменяет самокотроль для документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/ControlChange")]
        public IHttpActionResult ControlChange([FromBody]ControlChange model)
        {
            Action.Execute(EnumDocumentActions.ControlChange, model);
            var res = new JsonResult(true, this);
            return res;
        }

        /// <summary>
        /// Изменяет параметры контроля направлен для исполнения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/SendForExecutionChange")]
        public IHttpActionResult SendForExecutionChange([FromBody]ControlChange model)
        {
            Action.Execute(EnumDocumentActions.SendForExecutionChange, model);
            var res = new JsonResult(true, this);
            return res;
        }
        /// <summary>
        /// Изменяет параметры контроля направлен для исполнения(отв. исполнитель)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/SendForResponsibleExecutionChange")]
        public IHttpActionResult SendForResponsibleExecutionChange([FromBody]ControlChange model)
        {
            Action.Execute(EnumDocumentActions.SendForResponsibleExecutionChange, model);
            var res = new JsonResult(true, this);
            return res;
        }

        /// <summary>
        /// Изменяет параметры контроля, регулируемые исполнителем
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/ControlTargetChange")]
        public IHttpActionResult ControlTargetChange([FromBody]ControlTargetChange model)
        {
            Action.Execute(EnumDocumentActions.ControlTargetChange, model);
            var res = new JsonResult(true, this);
            return res;
        }






    }
}
