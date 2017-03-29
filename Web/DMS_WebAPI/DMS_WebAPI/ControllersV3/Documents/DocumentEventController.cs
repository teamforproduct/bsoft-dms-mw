using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. События.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentEventController : ApiController
    {
        /// <summary>
        /// Возвращает список событий
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [DimanicAuthorize("R")]
        [Route(Features.Events + "/Main")]
        [ResponseType(typeof(List<FrontDocumentEvent>))]
        public async Task<IHttpActionResult> PostGetList([FromBody]IncomingBase model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                if (model == null) model = new IncomingBase();
                if (model.Filter == null) model.Filter = new FilterBase();
                if (model.Paging == null) model.Paging = new UIPaging();

                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var items = docProc.GetDocumentEvents(context, model.Filter, model.Paging);
                var res = new JsonResult(items, this);
                res.Paging = model.Paging;
                return res;
            });
        }

        /// <summary>
        /// Возвращает событие по ИД
        /// </summary>
        /// <param name="Id">ИД события</param>
        /// <returns>Событие</returns>
        [HttpGet]
        [Route(Features.Events + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentEvent))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var item = docProc.GetDocumentEvent(context, Id);
                var res = new JsonResult(item, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает меню для работы с документом по событию
        /// </summary>
        /// <param name="Id">ИД события</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Events + "/{Id:int}" + "/Actions")]
        [ResponseType(typeof(List<InternalDictionaryPositionWithActions>))]
        public async Task<IHttpActionResult> Actions([FromUri]int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<ICommandService>();
                var items = docProc.GetDocumentActions(context, Id, Id);
                var res = new JsonResult(items, this);
                return res;
            });
        }

        /// <summary>
        /// Отмечает для пользователя ивенты как прочтенные
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Events + "/MarkAsRead")]
        public async Task<IHttpActionResult> MarkDocumentEventAsRead([FromBody]MarkDocumentEventAsRead model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.MarkDocumentEventAsRead, model);
                var res = new JsonResult(true, this);
                return res;
            });
        }

        /// <summary>
        /// Отправляет сообщение группе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Events + "/SendMessage")]
        public async Task<IHttpActionResult> SendMessage([FromBody]SendMessage model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.SendMessage, model, model.CurrentPositionId);
                var res = new JsonResult(true, this);
                return res;
            });
        }

        /// <summary>
        /// Добавляет примечание
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Events + "/AddNote")]
        public async Task<IHttpActionResult> AddNote([FromBody]AddNote model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.AddNote, model, model.CurrentPositionId);
                var res = new JsonResult(true, this);
                return res;
            });
        }

        /// <summary>
        /// Направляет документ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Events + "/SendDocument")]
        public async Task<IHttpActionResult> SendDocument([FromBody]List<AddDocumentSendList> model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.First().CurrentPositionId);
                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var tmpItem = (Dictionary<int, string>)docProc.ExecuteAction(EnumDocumentActions.SendDocument, ctx, model);
                var res = new JsonResult(tmpItem, !tmpItem.Any(), this);
                return res;
            });
        }

    }
}
