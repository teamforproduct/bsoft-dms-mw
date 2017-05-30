using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
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
using System.Threading.Tasks;
using System.Web;
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
    public class DocumentWaitController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var item = docProc.GetDocumentEvent(context, Id);
            var res = new JsonResult(item, this);
            return res;
        }

        /// <summary>
        /// Возвращает список контролей
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Waits + "/Main")]
        [ResponseType(typeof(List<FrontDocumentWait>))]
        public async Task<IHttpActionResult> PostGetList([FromBody]IncomingBase model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   if (model == null) model = new IncomingBase();
                   if (model.Filter == null) model.Filter = new FilterBase();
                   if (model.Paging == null) model.Paging = new UIPaging();

                   var docProc = DmsResolver.Current.Get<IDocumentService>();
                   var items = docProc.GetDocumentWaits(context, model.Filter, model.Paging);
                   var res = new JsonResult(items, this);
                   res.Paging = model.Paging;
                   return res;
               });
        }

        /// <summary>
        /// Возвращает контроль по ИД
        /// </summary>
        /// <param name="Id">ИД контроля</param>
        /// <returns>Событие</returns>
        [HttpGet]
        [Route(Features.Waits + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentWait))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет самокотроль для документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Waits + "/ControlOn")]
        public async Task<IHttpActionResult> ControlOn([FromBody]ControlOn model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.ControlOn, model, model.CurrentPositionId);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Добавляет прошение о переносе сроков исполнения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/AskPostponeDueDate")]
        public async Task<IHttpActionResult> AskPostponeDueDate([FromBody]AskPostponeDueDate model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.AskPostponeDueDate, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Добавляет контроль о выполнении поручения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/MarkExecution")]
        public async Task<IHttpActionResult> MarkExecution([FromBody]SendEventMessage model)
        {
            model.ServerPath = Properties.Settings.Default.ServerPath;
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.MarkExecution, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }



        /// <summary>
        /// Регистрирует отказ по прошению о переносе сроков исполнения 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/CancelPostponeDueDate")]
        public async Task<IHttpActionResult> CancelPostponeDueDate([FromBody]SendEventMessage model)
        {
            model.ServerPath = Properties.Settings.Default.ServerPath;
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.CancelPostponeDueDate, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Регистрирует отказ в приеме результата
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/RejectResult")]
        public async Task<IHttpActionResult> RejectResult([FromBody]SendEventMessage model)
        {
            model.ServerPath = Properties.Settings.Default.ServerPath;
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.RejectResult, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Регистрирует прием результата
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/AcceptResult")]
        public async Task<IHttpActionResult> AcceptResult([FromBody]ControlOff model)
        {
            model.ServerPath = Properties.Settings.Default.ServerPath;
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.AcceptResult, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Регистрирует отмену поручения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/CancelExecution")]
        public async Task<IHttpActionResult> CancelExecution([FromBody]ControlOff model)
        {
            model.ServerPath = Properties.Settings.Default.ServerPath;
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.CancelExecution, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }



        /// <summary>
        /// Снимает самоконтроль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/ControlOff")]
        public async Task<IHttpActionResult> ControlOff([FromBody]ControlOff model)
        {
            model.ServerPath = Properties.Settings.Default.ServerPath;
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.ControlOff, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }


        /// <summary>
        /// Изменяет самокотроль для документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/ControlChange")]
        public async Task<IHttpActionResult> ControlChange([FromBody]ControlChange model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.ControlChange, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Изменяет параметры контроля направлен для исполнения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/SendForExecutionChange")]
        public async Task<IHttpActionResult> SendForExecutionChange([FromBody]ControlChange model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.SendForExecutionChange, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }
        /// <summary>
        /// Изменяет параметры контроля направлен для исполнения(отв. исполнитель)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/SendForResponsibleExecutionChange")]
        public async Task<IHttpActionResult> SendForResponsibleExecutionChange([FromBody]ControlChange model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.SendForResponsibleExecutionChange, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Изменяет параметры контроля, регулируемые исполнителем
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Waits + "/ControlTargetChange")]
        public async Task<IHttpActionResult> ControlTargetChange([FromBody]ControlTargetChange model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.ControlTargetChange, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }






    }
}
