using System.Web.Http;
using System.Web.Http.Description;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using BL.Model.DocumentCore.IncomingModel;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "DocumentEvents")]
    public class DocumentEventsController : ApiController
    {
        /// <summary>
        /// Получение списка ивентов 
        /// </summary>
        /// <param name="filter">модель фильтра ивентов</param>
        /// <param name="paging">paging</param>
        /// <returns>список ивентов</returns>
        [ResponseType(typeof(List<FrontDocumentEvent>))]
        public IHttpActionResult Get([FromUri] FilterBase filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var events = docProc.GetDocumentEvents(ctx, filter, paging);
            var res = new JsonResult(events, this);
            res.Paging = paging;
            return res;
        }


        /// <summary>
        /// Получение списка ивентов 
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns>Список ивентов</returns>
        [HttpPost]
        [Route("GetList")]
        [ResponseType(typeof(List<FrontDocumentWait>))]
        public IHttpActionResult PostGetList([FromBody]IncomingBase model)
        {
            if (model == null) model = new IncomingBase();
            if (model.Filter == null) model.Filter = new FilterBase();
            if (model.Paging == null) model.Paging = new UIPaging();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var events = docProc.GetDocumentEvents(ctx, model.Filter, model.Paging);
            var res = new JsonResult(events, this);
            res.Paging = model.Paging;
            return res;
        }


        /// <summary>
        /// Получение ивента по ИД
        /// </summary>
        /// <param name="id">ИД ивента</param>
        /// <returns>Ивент</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var evt = docProc.GetDocumentEvent(ctx, id);
            var res = new JsonResult(evt, this);
            return res;
        }
/*
        /// <summary>
        /// получение всех ивентов по одному документу
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <param name="paging"> paging</param>
        /// <returns>соответствующая страница списка ивентов документа</returns>
        [Route("GetEventsByDocument")]
        [HttpGet]
        public IHttpActionResult GetEventsByDocument(int id,[FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var events = docProc.GetEventsForDocument(ctx, id, paging);
            var res = new JsonResult(events, this);
            return res;
        }
        */
    }
}