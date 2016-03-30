using System.Web.Http;
using System.Web.Http.Description;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentEventsController : ApiController
    {
        /// <summary>
        /// Получение списка ивентов 
        /// </summary>
        /// <param name="filter">модель фильтра ивентов</param>
        /// <param name="paging">paging</param>
        /// <returns>список ивентов</returns>
        [ResponseType(typeof(FrontDocumentEvent))]
        public IHttpActionResult Get([FromUri] FilterDocumentEvent filter, [FromUri]UIPaging paging)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var events = docProc.GetDocumentEvents(cxt, filter, paging);
            var res = new JsonResult(events, this);
            res.Paging = paging;
            return res;
        }


        /// <summary>
        /// Получение ивента по ИД
        /// </summary>
        /// <param name="id">ИД ивента</param>
        /// <returns>Ивент</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var evt = docProc.GetDocumentEvent(cxt, id);
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var events = docProc.GetEventsForDocument(cxt, id, paging);
            var res = new JsonResult(events, this);
            return res;
        }
        */
    }
}